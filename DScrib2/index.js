var UserIdToken = null;
var gExplorer = null;

var KeyCodes = {
  ENTER: 13
}

var Product = Backbone.Model.extend({
  idAttribute: "ID",
  view: null,
  foldedView: null,
  busy: false,
  formattedDate: null,

  initialize: function () {
    Backbone.Model.prototype.initialize.apply(this, arguments)
    this.listenTo(this, 'request', this.onRequest)
    this.listenTo(this, 'sync', this.onSync)
    this.listenTo(this, 'error', this.onError)
  },

  onRequest: function() { this.busy = true },

  onSync: function () {
    this.busy = false

    if (this.get('Unsave')) {
      this.unset('Unsave', {silent: true})
      this.unset('ID')
    }
  },

  onError: function() { this.busy = false },

  // id (from link), name, link-pretty-part (pretty part, ID part), a review
  // id may eventually come from our database.
  fetchReview: function () {
    if (this.busy || this.isRetrieved()) return;

    this.trigger('request')
    $.get({
      url: '/reviews/fetch',
      data: {
        linkSlug: this.get('Slug'),
        productID: this.get('AmazonID')
      },
      dataType: 'JSON',
      success: _.bind(function (data) {
        var review = ((data === null) ? { Date: 'n/a', Text: '(unable to retrieve)' } : data)
        this.set(review)
        this.trigger('sync')
      }, this),
      error: _.bind(function (jqXhr, textStatus, errorThrown) {
        review = { Date: 'n/a', Text: '(unable to retrieve)' }
        this.set(review)
        this.trigger('error')
      }, this)
    })
  },

  unsave: function () {
    this.save({'Unsave': true})
  },

  isRetrieved: function () {
    return typeof (this.get('Text')) !== 'undefined'
  },

  getFormattedDate: function () {
    if (!this.has('Date')) return "";

    if (this.formattedDate === null) {
      this.formattedDate = (this.get('Date') === 'n/a') ? 'n/a' : moment(this.get('Date')).format('MMMM Do YYYY')
    }

    return this.formattedDate;
  },

  getView: function () {
    if (this.view === null) {
      this.view = new ProductView({ model: this }).render()
    }

    return this.view;
  },

  getFoldedView: function () {
    if (this.foldedView === null) {
      this.foldedView = new ProductView({ model: this, folded: true }).render()
    }

    return this.foldedView;
  }
})

var ProductView = Backbone.View.extend({
  tagName: 'tr',

  busy: false,

  events: {
    'click .review-body-toggle': 'onToggle',
    'click .fetch-action': 'onFetch',
    'click i.save-action': 'onSaveRequest',
    'click i.delete-action': 'onDestroyRequest'
  },

  initialize: function (opts) {
    Backbone.View.prototype.initialize.apply(this, arguments);
    this.folded = (opts.folded === true)
    this.listenTo(this.model, 'change', this.onChange)
    this.listenTo(this.model, 'error', this.onError)
    this.listenTo(this.model, 'sync', this.onSync)
    this.listenTo(this.model, 'request', this.onRequest)
  },

  onFetch: function (e) {
    e.preventDefault()
    this.model.fetchReview()
    return false
  },

  onToggle: function () {
    if (this.folded) {
      this.folded = false
      this.render()
    } else {
      this.folded = true
      this.render()
    }    
  },

  onSaveRequest: function () {
    this.model.save()
  },

  onDestroyRequest: function () {
    this.model.unsave()
  },

  onChange: function () {
    this.render()
  },

  onRequest: function () {
    this.lastError = ""
    this.render()
  },

  onSync: function () {
    this.render()
  },

  onError: function () {
    this.lastError = "An error occured while fetching information."
    this.render()
  },

  render: function () {
    this.$el.empty()

    var statusIcons = (typeof (this.model.get('ID')) !== 'undefined'
      ? ' <i class="fas fa-star delete-action action" area-label="Forget this review" title="Forget this review"></i>'
      : ' <i class="far fa-star save-action action" area-label="Save this review" title="Save this review"></i>')
    var busyIcon = (this.model.busy ? ' <i class="fas fa-spinner fa-spin"></i>' : '')
    var dateStr = '<td>' + this.model.getFormattedDate() + '</td>'

    if (this.model.isRetrieved()) {
      if (!this.folded) {
        this.$el.append($('<td>'
          + '<span class="review-body-toggle" title="Hide Review">' + this.model.get('Name') + ' <i class="far fa-envelope-open"></i></span>'
          + statusIcons
          + busyIcon
          + '<br/>' + this.model.get('Text')
          + '</td>'
          + dateStr
        ))
      } else {
        this.$el.append($('<td>'
          + '<span class="review-body-toggle" title="Show Review">' + this.model.get('Name') + ' <i class="far fa-envelope"></i></span>'
          + statusIcons
          + busyIcon
          + '</td>'
          + dateStr
        ))
      }
    } else {
      this.$el.append($('<td>'
        + '<a href="#" class="fetch-action"  title="Fetch Review">'
        + this.model.get('Name')
        + ' <i class="far fa-envelope fetch-action"></i>'
        + '</a>'
        + busyIcon
        + '</td><td></td>')
      )
    }

    this.$el.append($('<td><a href="https://www.amazon.com/' + this.model.get('Slug') + "/dp/" + this.model.get('AmazonID') + '" target="_blank">Visit</a></td>'))
    return this
  }
})

var Products = Backbone.Collection.extend({
  url: '/reviews',
  model: Product
})

var Explorer = Backbone.View.extend({

  events: {
    'change input[name=search]': 'onChange',
    'keyup input[name=search]': 'onChange',
    'submit form.search-form': 'onSearch',
  },

  search: "",
  searching: false,
  results: new Products(),
  loggedIn: false,
  userID: null,
  lastError: null,

  initialize: function (options) {
    Backbone.View.prototype.initialize.apply(this, arguments);

    if ('userID' in options && options.userID !== null) {
      this.userID = options.userID;
      this.loggedIn = true;
    }
    this.listenTo(this.results, 'reset', this.onResultsChanged)
  },

  onChange: function (e) {
    this.search = e.target.value;
  },

  ensureLoggedIn: function () {
    if (UserIdToken !== null && !this.loggedIn) {
      $.ajax({
        url: '/Sessions/TokenLogin',
        method: 'POST',
        dataType: 'JSON',
        data: {
          idToken: UserIdToken
        },
        success: _.bind(function (data) {
          if ('ID' in data) {
            this.userID = data.ID;
            this.loggedIn = true;
            this.render()
          } else {
            this.lastError = "A very unexpected error occurred while trying to log you in. This is likely a bug. ☹"
            this.render()
          }
        }, this),
        error: _.bind(function (jqXhr, textStatus, errorThrown) {
          this.lastError = "An error occurred while trying to log you in."
          this.render()
        }, this)
      })
    }
  },

  onResultsChanged: function (e) {
    this.render();
  },
  
  onSearch: function(e) {    
    e.preventDefault()
    this.lastError = null;

    if (!this.loggedIn) {
      this.lastError = "You must be logged in to use this feature.";
      this.render()
      return;
    }

    this.searching = true
    this.results.reset()
    $.get({
      url: '/search',
      data: {
        q: this.search
      },
      dataType: 'JSON',
      success: _.bind(function (data) {
        this.searching = false
        this.results.reset(data); // will call render...kinda weird.
      }, this),
      error: _.bind(function () {
        this.searching = false
        this.lastError = "An error occurred while searching."
        this.render()
      }, this)
    })
    this.render()
  },

  render: function () {

    var infoBox = ""
    if (this.lastError !== null) {
      infoBox = $('<div class="alert alert-danger primary-error role="alert">' + this.lastError + '</div>')
    } else if (this.loggedIn === false) {
      infoBox = $('<div class="alert alert-warning" role="alert">You must be logged in to use this site.</div>')
    }

    var searchForm = $('<form class="form-inline search-form">'
      + '<fieldset ' + (this.searching ? 'disabled' : '') + '>'
      + '<input type="text" name="search" placeholder="Product to Search For" class="form-control mb-2 mr-sm-2"/>'
      + '<button type="submit" class="btn btn-primary mb-2 mr-sm-2">Search</button>'
      + (this.searching ? '<i class="fas fa-spinner fa-spin"></i>' : '')
      + '</fieldset></form>'
    )
    searchForm.find('input[name=search]').val(this.search)

    var tableArea = null
    if (!this.searching && this.results.length > 0) {
      var tableArea = $(
        "<table class='table table-bordered'><thead class='thead-dark'>"
        + '<tr><th>Product</th><th>Review Date</th><th>Link</th></tr>'
        + '</thead><tbody>'
        + '</tbody></table >'
      );

      var tbody = tableArea.find('tbody')
      this.results.each(function (pi) {
        tbody.append(pi.getView().el)
      });
    }

    this.$el.empty()
    this.$el.append(infoBox)
    this.$el.append(searchForm)
    this.$el.append(tableArea)
    return this
  },

  ready: function() {
    this.$('input[name=search]').focus()
  },

  prepare: function () {
    this.render()
    if (this.loggedIn) {
      this.ready()
    }
  }
})

function onSignIn(googleUser) {
  var id_token = googleUser.getAuthResponse().id_token;
  //console.log("ID Token: " + id_token);
  UserIdToken = id_token
  if (gExplorer) {
    gExplorer.ensureLoggedIn();
  }
};

var ReviewsView = Backbone.View.extend({
  initialize: function (opts) {
    Backbone.View.prototype.initialize.apply(this, arguments)
    this.reviews = new Products(opts.models)
  },

  render: function () {

    var tableArea = null
    if (this.reviews.length > 0) {
      tableArea = $('<table class="table table-bordered"><thead class="thead-dark"><tr><th>Product</th><th>Review Date</th><th>Link</th></tr></thead><tbody></tbody></table>');
      var tbody = tableArea.find('tbody');
      this.reviews.each(function (pi) {
        tbody.append(pi.getFoldedView().el);
      })
    } else {
      tableArea = $('<div>No saved reviews.</div>')
    }

    this.$el.empty()
    this.$el.append(tableArea)
    return this
  }
})
/********** Boot the App *********/

$(function () {


  $(document).ready(function () {
    var securityToken = $('form.csrf-token-form [name=__RequestVerificationToken]').val();
    $(document).ajaxSend(function (event, jqXhr, opt) {
      if ('type' in opt && opt.type !== 'GET') {
        jqXhr.setRequestHeader('__RequestVerificationToken', securityToken)
      }
    });
  });

  $(function () {
    $('[data-toggle="popover"]').popover()
  })

  if (location.pathname === '/' || location.pathname === '/Home/Index') {
    gExplorer = new Explorer({
      el: $('.main-app').first(),
      userID: __userID
    })
    gExplorer.prepare()
  } else if (location.pathname === '/me/reviews') {
    var rv = new ReviewsView({
      el: $('.reviews-app').first(),
      models: __bootstrap
    });
    rv.render()
  }

})
