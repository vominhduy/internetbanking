import Vue from 'vue'
import './plugins/bootstrap-vue'
import App from './App.vue'
import router from './router'
import {store} from './store/store'
import helper from './helper/helper'

Vue.config.productionTip = false

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuthen)) {
    if (!store.getters.loggedIn) {
      next({
        name: 'Login',
      })
    } 
    // if logged in, check role
    else {
      let authorize = to.meta.requiresRole;
      let token = localStorage.getItem('access_token');
      let userInfo = helper.parseJwt(token);
      
      if(authorize){
        if(!token || !userInfo){
          next({
            name: 'Login',
          })
        }

        if (authorize.length && authorize.includes(userInfo.role)) {
          next()
        }
        else{
          next({ path: from.path })
        }
      }
      else{
        next()
      }
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
