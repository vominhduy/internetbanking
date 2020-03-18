import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from 'axios'
import AdminLayout from '../layouts/AdminLayout.vue'
import LoginLayout from '../layouts/LoginLayout.vue'
import EmployeeLayout from '../layouts/EmployeeLayout.vue'
import UserLayout from '../layouts/UserLayout.vue'

import EmployeeHome from '../views/Employee/Home.vue'

import UserHome from '../views/User/Home.vue'

import AdminHome from '../views/Admin/Home.vue'

import Home from '../views/Home.vue'
import Login from '../views/Auth/Login.vue'

import CreateUser from '../views/Employee/CreateUser.vue'
import PayIn from '../views/Employee/PayIn.vue'
import HistoryIn from '../views/Employee/HistoryIn.vue'
import HistoryOut from '../views/Employee/HistoryOut.vue'
import HistoryDeptIn from '../views/Employee/HistoryDeptIn.vue'
import HistoryDeptOut from '../views/Employee/HistoryDeptOut.vue'
import remittance from '../views/User/remittance.vue'


import { BootstrapVueIcons } from 'bootstrap-vue'

Vue.use(BootstrapVueIcons)

Vue.use(VueRouter)

axios.defaults.baseURL = "https://localhost:44396/api/";

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { layout: LoginLayout }
  },
  {
    path: '/employee/',
    name: 'EmployeeHome',
    component: EmployeeHome,
    meta: { layout: EmployeeLayout }
  },
  {
    path: '/user/',
    name: 'UserHome',
    component: UserHome,
    meta: { layout: UserLayout }
  },
  {
    path: '/admin/',
    name: 'AdminHome',
    component: AdminHome,
    meta: { layout: AdminLayout }
  },
  {
    path: '/employee/create-user',
    name: 'CreateUser',
    component: CreateUser,
    meta: { layout: EmployeeLayout }
  },
  {
    path: '/employee/pay-in',
    name: 'PayIn',
    component: PayIn,
    meta: { layout: EmployeeLayout }
  }
  ,
  {
    path: '/employee/historys/payin',
    name: 'HistoryIn',
    component: HistoryIn,
    meta: { layout: EmployeeLayout }
  }
  ,
  {
    path: '/employee/historys/payout',
    name: 'HistoryOut',
    component: HistoryOut,
    meta: { layout: EmployeeLayout }
  }
  ,
  {
    path: '/employee/historys/deptin',
    name: 'HistoryDeptIn',
    component: HistoryDeptIn,
    meta: { layout: EmployeeLayout }
  }
  ,
  {
    path: '/employee/historys/deptout',
    name: 'HistoryDeptOut',
    component: HistoryDeptOut,
    meta: { layout: EmployeeLayout }
  }
  ,
  {
    path: '/user/remittance',
    name: 'remittance',
    component: remittance,
    meta: { layout: UserLayout }
  },
]

const router = new VueRouter({
  routes
})

export default router
