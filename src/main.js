import Vue from 'vue'
import App from './App.vue'
import moment from 'moment'

Vue.config.productionTip = false

Vue.filter('formatDate', function(value) {
  if (value) {
    return moment(String(value)).format('YYYY-MM-DD hh:mm')
  }
})

new Vue({
  render: h => h(App),
}).$mount('#app')
