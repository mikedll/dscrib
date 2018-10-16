
var KeyCodes = {
  ENTER: 13
}

var Product = Backbone.Model.extend({
  view: null,
  sassy: false,

  turnSassy: function () {
    this.set({ sassy: true });
  },

  reviewShort: function () {
    var r = this.get('review');
    if (typeof(r) === 'undefined' || r.length === 0) return "";

    return r.substr(0, Math.min(r.length, 10)) + ((r.length > 10) ? "…" : '');
  },

  // id (from link), name, link-pretty-part (pretty part, ID part), a review
  // id may eventually come from our database.
  fetchReview: function () {
    console.log("getReview called.")
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
    'click': 'sassify'
  },

  initialize: function () {
    Backbone.View.prototype.initialize.apply(this, arguments);
    this.listenTo(this.model, 'change', this.onChange)
  },

  onChange: function () {
    this.render()
  },

  sassify: function () { this.model.turnSassy(); },

  render: function () {

    this.$el.empty()
    if (this.model.get('sassy')) {
      this.$el.append($('<td colspan="2"> count=' + this.renderCount + ': ' + 'LOL!</td>'))
    } else {
      this.$el.append($('<td> count=' + this.renderCount + ': ' + this.model.get('name') + '</td><td> ' + this.model.reviewShort() + '</td > '))
    }
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
      this.results.each(function (pi) {
        tbody.append(pi.getView().el)
      });
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
