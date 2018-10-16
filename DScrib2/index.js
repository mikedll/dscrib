
var KeyCodes = {
  ENTER: 13
}

var ProductInfo = Backbone.Model.extend({
  // id (from link), name, link-pretty-part (pretty part, ID part), a review
  // id may eventually come from our database.
  getReview: function () {
    console.log("getReview called.")
  }
})

var Products = Backbone.Collection.extend({
  model: ProductInfo
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

  render: function() {
    var html = ''
    html += '<input type="text" name="search" placeholder="Product to Search For"  value="alexa"/>'

    var tableEls = "<table class='table table-bordered'><thead>";
    tableEls += '<tr><th>Product</th><th>...dunno</th></tr>';
    tableEls += '</thead><tbody>';
    if (this.results.length > 0) {
      tableEls += this.results.map(function (pi) {
        return '<tr><td>' + pi.get('name') + '</td><td></td></tr>';
      }).join('');
    } else if (this.searching) {
      tableEls += '<tr><td colspan="2">Searching...</td></tr>'
    }
    tableEls += '</tbody></table >';
    html += tableEls;
    
    this.$el.html(html)
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
