var UserIdToken = null;
var gExplorer = null;

var KeyCodes = {
  ENTER: 13
}

var Product = Backbone.Model.extend({
  view: null,
  foldedView: null,
  busy: false,
  formattedDate: null,

  // id (from link), name, link-pretty-part (pretty part, ID part), a review
  // id may eventually come from our database.
  fetchReview: function () {
    if (this.busy || this.isRetrieved()) return;
    this.busy = true;
    $.get({
      url: '/reviews/show',
      data: {
        linkSlug: this.get('Slug'),
        productID: this.get('AmazonID')
      },
      dataType: 'JSON',
      success: _.bind(function (data) {
        var review = ((data === null) ? { Date: 'n/a', Text: '(unable to retrieve)' } : data)
        this.set(review)
      }, this),
      error: _.bind(function (jqXhr, textStatus, errorThrown) {
        // Unusual, null review fetched.
        if (jqXhr.status === 200) {
          review = { Date: 'n/a', Text: '(unable to retrieve)' }
          this.set({ 'review': review })
        }
      }, this),
      complete: _.bind(function () {
        this.busy = false
      }, this)
    })
  },

  isRetrieved: function () {
    return typeof (this.get('Text')) !== 'undefined'
  },

  getFormattedDate: function () {
    if (this.formattedDate === null) {
      this.formattedDate = (this.get('Date') == 'n/a') ? 'n/a' : moment(this.get('Date')).format('MMMM Do YYYY')
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

  events: {
    'click': 'onToggle'
  },

  initialize: function (opts) {
    Backbone.View.prototype.initialize.apply(this, arguments);
    this.folded = (opts.folded === true)
    this.listenTo(this.model, 'change', this.onChange)
  },

  onToggle: function () {
    if (!this.model.isRetrieved()) {
      this.model.fetchReview();
    } else if (this.folded) {
      this.folded = false
      this.render()
    } else {
      this.folded = true
      this.render()
    }    
  },

  onChange: function () {
    this.render()
  },

  render: function () {
    this.$el.empty()
    if (typeof (this.model.get('Text')) !== 'undefined') {
      if (!this.folded) {
        this.$el.append($('<td><span class="product-name">' + this.model.get('Name') + '</span><br/> ' + this.model.get('Text') + '</td><td>' + this.model.getFormattedDate() + '</td>'))
      } else {
        this.$el.append($('<td>' + this.model.get('Name') + '</td><td>' + this.model.getFormattedDate() + '</td>'))
      }
    } else {
      this.$el.append($('<td>' + this.model.get('Name') + '</td><td></td>'))
    }

    this.$el.append($('<td><a href="https://www.amazon.com/' + this.model.get('Slug') + "/dp/" + this.model.get('AmazonID') + '" target="_blank">Visit</a>'))
    return this
  }
})

var Products = Backbone.Collection.extend({
  model: Product
})

var Explorer = Backbone.View.extend({

  events: {
    'change input[name=search]': 'onSearchChange',
    'keyup input[name=search]': 'onKeyUp'
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
            this.ready()
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

  onSearchChange: function(e) {
    this.search = e.target.value;
  },

  onResultsChanged: function (e) {
    this.render();
  },
  
  onKeyUp: function(e) {    
    if (e.keyCode === KeyCodes.ENTER) {
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
          q: e.target.value
        },
        dataType: 'JSON',
        success: _.bind(function (data) {
          this.searching = false
          this.results.reset(data);
        }, this)
      })
    }
  },

  render: function () {
    var infoBox = ""
    if (this.lastError !== null) {
      infoBox = $('<div class="alert alert-danger primary-error role="alert">' + this.lastError + '</div>')
    } else if (this.loggedIn === false) {
      infoBox = $('<div class="alert alert-warning" role="alert">You must be logged in to use this site.</div>')
    }

    var searchBoxArea = $('<input type="text" name="search" placeholder="Product to Search For"  value="alexa"/>')
    var menu = null;
    if (this.loggedIn === true) {
      menu = $('<div><a href="/me/reviews">Your Saved Reviews</a></div>')
    }

    var table = $(
      "<table class='table table-bordered'><thead class='thead-dark'>"
      + '<tr><th>Product</th><th>Review Date</th><th>Link</th></tr>'
      + '</thead><tbody>'
      + '</tbody></table >'
    );

    var tbody = table.find('tbody')
    if (this.results.length > 0) {
      var found = 0
      this.results.each(function (pi) {
        found += 1
        tbody.append(pi.getView().el)
      });
      console.log("I think I added " + found + " records' views to the root view.")
    } else if (this.searching) {
      tbody.append($('<tr><td colspan="3">Searching...</td></tr>'));
    }

    this.$el.empty()
    this.$el.append(infoBox)
    this.$el.append(searchBoxArea)
    this.$el.append(menu)
    this.$el.append(table)
    return this
  },

  ready: function() {
    this.$('input[name=search]').focus()
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
    var link = $('<div><a href="/">Home</a></div>')
    var table = $('<table class="table table-bordered"><thead class="thead-dark"><tr><th>Product</th><th>Review Date</th><th>Link</th></tr></thead><tbody></tbody></table>');
    var tbody = table.find('tbody');
    this.reviews.each(function (pi) {
      tbody.append(pi.getFoldedView().el);
    })

    this.$el.empty()
    this.$el.append(link)
    this.$el.append(table)
    return this
  }
})
/********** Boot the App *********/

$(function () {

  if (location.pathname === '/' || location.pathname === '/Home/Index') {
    gExplorer = new Explorer({
      el: $('.main-app').first(),
      userID: __userID
    })
    gExplorer.render()
  } else if (location.pathname === '/me/reviews') {
    var rv = new ReviewsView({
      el: $('.reviews-app').first(),
      models: __bootstrap
    });
    rv.render()
  }

})
