import Vue from 'vue'
import './plugins/bootstrap-vue'
import App from './App.vue'
import router from './router'
import {store} from './store/store'

Vue.config.productionTip = false

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuthen)) {
    if (!store.getters.loggedIn) {
      next({
        name: 'Login',
      })
    } else {
      next()
    }
  } else {
    next()
  }
})

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
