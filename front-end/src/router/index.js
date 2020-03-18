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

Vue.use(VueRouter)

axios.defaults.baseURL = "http://localhost:5000/api/";

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
]

const router = new VueRouter({
  routes
})

export default router
