import { createApp } from 'vue'
import App from './App.vue'
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";
//import moment from 'moment'

// Vue.config.productionTip = false

// Vue.filter('formatDate', function(value) {
//   if (value) {
//     return moment(String(value)).format('YYYY-MM-DD hh:mm')
//   }
// })

// new Vue({
//   render: h => h(App),
// }).$mount('#app')

const app = createApp(App)
app.mount('#app')