import { createApp } from 'vue'
import {createRouter, createWebHashHistory} from 'vue-router'
import App from './App.vue'
import HomeRegister from './HomeRegister.vue'
import AboutInfo from './AboutInfo.vue'
import ScoreBoard from './ScoreBoard.vue'
import moment from 'moment'
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";


// 2. Define some routes
// Each route should map to a component.
// We'll talk about nested routes later.
const routes = [
  { path: '/', component: HomeRegister },
  { path: '/about', component: AboutInfo },
  { path: '/scoreboard/:board', component: ScoreBoard}
]

// 3. Create the router instance and pass the `routes` option
// You can pass in additional options here, but let's
// keep it simple for now.
const router = createRouter({
  // 4. Provide the history implementation to use. We are using the hash history for simplicity here.
  history: createWebHashHistory(),
  routes, // short for `routes: routes`
})

const app = createApp(App)
app.use(router)

app.config.globalProperties.moment = moment;

app.mount('#app')