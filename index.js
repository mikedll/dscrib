
var KeyCodes = {
  ENTER: 13
}

var Explorer = Backbone.View.extend({

  events: {
    'change input[name=search]': 'onChange',
    'keyup input[name=search]': 'onKeyUp'
  },

  search: "",
  results: "",

  onChange: function(e) {
    this.search = e.target.value;
  },
  
  onKeyUp: function(e) {    
    if(e.keyCode === KeyCodes.ENTER) {
      e.preventDefault()
      console.log("submitted with " + e.target.value)

      $.get({
        url: 'https://www.amazon.com/robots.txt',
        success: _.bind(function(data) {
          this.results = data
          this.render()
        }, this)
      })
    }
  },

  render: function() {
    var html = ''
    html += '<input type="text" name="search" placeholder="Product to Search For"  value="alexa"/>'
    if(this.results !== '') {
      html += '<div class="results">' + this.results + '</div>'
    }
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
