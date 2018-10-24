var UserIdToken = null;
var gExplorer = null;

var KeyCodes = {
  ENTER: 13
}

var Product = Backbone.Model.extend({
  view: null,
  busy: false,

  // id (from link), name, link-pretty-part (pretty part, ID part), a review
  // id may eventually come from our database.
  fetchReview: function () {
    if (this.busy || typeof(this.get('review')) !== 'undefined') return;
    this.busy = true;
    $.get({
      url: '/reviews',
      data: {
        linkSlug: this.get('linkSlug'),
        productID: this.get('productID')
      },
      dataType: 'JSON',
      success: _.bind(function (data) {
        var review;
        if (data === null) {
          review = {reviewDate: 'n/a', review: '(unable to retrieve)'}
        }
        review = _.extend({}, data, { 'reviewDate': moment(data.reviewDate).format('MMMM Do YYYY') })
        this.set({ 'review': review })
      }, this),
      error: _.bind(function (jqXhr, textStatus, errorThrown) {
        // Unusual, null review fetched.
        if (jqXhr.status === 200) {
          review = { reviewDate: 'n/a', review: '(unable to retrieve)' }
          this.set({ 'review': review })
        }
      }, this),
      complete: _.bind(function () {
        this.busy = false
      }, this)
    })
  },

  reviewShort: function () {
    var r = this.get('review');
    if (typeof(r) === 'undefined' || r.review.length === 0) return "";

    return r.review.substr(0, Math.min(r.review.length, 10)) + ((r.review.length > 10) ? "…" : '');
  },

  getView: function () {
    if (this.view === null) {
      this.view = new ProductView({ model: this }).render()
    }

    return this.view;
  }
})

var ProductView = Backbone.View.extend({
  tagName: 'tr',

  events: {
    'click': 'getReview'
  },

  initialize: function () {
    Backbone.View.prototype.initialize.apply(this, arguments);
    this.listenTo(this.model, 'change', this.onChange)
  },

  getReview: function () { this.model.fetchReview(); },

  onChange: function () {
    this.render()
  },

  render: function () {
    this.$el.empty()
    if (typeof(this.model.get('review')) !== 'undefined') {
      this.$el.append($('<td><span class="product-name">' + this.model.get('name') + '</span><br/> ' + this.model.get('review').review + '</td><td>' + this.model.get('review').reviewDate + '</td>'))
    } else {
      this.$el.append($('<td>' + this.model.get('name') + '</td><td></td>'))
    }

    this.$el.append($('<td><a href="https://www.amazon.com/' + this.model.get('linkSlug') + "/dp/" + this.model.get('productID') + '" target="_blank">Visit</a>'))
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
    var error = ""
    if (this.lastError !== null) {
      error = $('<div class="alert alert-danger primary-error">' + this.lastError + '</div>')
    }

    var searchBox = $('<input type="text" name="search" placeholder="Product to Search For"  value="alexa"/>')

    var table = $(
      "<table class='table table-bordered'><thead>"
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
    this.$el.append(error)
    this.$el.append(searchBox)
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

/********** Boot the App *********/

$(function() {
  gExplorer = new Explorer({
    el: $('.main-app').first()
  })
  gExplorer.render()
  gExplorer.ready()
})
