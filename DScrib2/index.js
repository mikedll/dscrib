
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
        var review = _.extend({}, data, { 'reviewDate': moment(data.reviewDate).format('MMMM Do YYYY, h:mm a') })
        this.set({ 'review': review })
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
  renderCount: 0,

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
    this.$el.append($('<td> count=' + this.renderCount + ': ' + this.model.get('name') + '</td><td> ' + this.model.reviewShort() + '</td > '))
    this.renderCount++;
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

  initialize: function (options) {
    Backbone.View.prototype.initialize.apply(this, arguments);

    this.listenTo(this.results, 'reset', this.onResultsChanged)
  },

  onSearchChange: function(e) {
    this.search = e.target.value;
  },

  onResultsChanged: function (e) {
    this.render();
  },
  
  onKeyUp: function(e) {    
    if(e.keyCode === KeyCodes.ENTER) {
      e.preventDefault()

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
    var searchBox = $('<input type="text" name="search" placeholder="Product to Search For"  value="alexa"/>')

    var table = $(
      "<table class='table table-bordered'><thead>"
      + '<tr><th>Product</th><th>...dunno</th></tr>'
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
      tbody.append($('<tr><td colspan="2">Searching...</td></tr>'));
    }

    this.$el.empty()
    this.$el.append(searchBox)
    this.$el.append(table)
    return this
  },

  ready: function() {
    this.$('input[name=search]').focus()
  } 
})

/********** Boot the App *********/

$(function() {
  var e = new Explorer({
    el: $('.main-app').first()
  })
  e.render()
  e.ready()
})
