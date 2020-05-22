import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from 'axios'
import AdminLayout from '../layouts/AdminLayout.vue'
import LoginLayout from '../layouts/LoginLayout.vue'
import EmployeeLayout from '../layouts/EmployeeLayout.vue'
import UserLayout from '../layouts/UserLayout.vue'
import constant from '../helper/constant'

import AddStaffLayout from '../layouts/AddStaffLayout.vue'
import EmployeeHome from '../views/Employee/Home.vue'

import UserHome from '../views/User/Home.vue'
import UserAccounts from '../views/User/UserAccounts.vue'
import UserContacts from '../views/User/Contacts.vue'
import UserHistories from '../views/User/History.vue'
import UserDeptreminders from '../views/User/Deptreminders.vue'

import AdminHome from '../views/Admin/Home.vue'
import AddStaff from '../views/Admin/AddStaff.vue'
import ListEmployee from '../views/Admin/ListEmployee.vue'
import DoisoatIN  from '../views/Admin/DoisoatIN.vue'
import DoisoatOUT from '../views/Admin/DoisoatOUT.vue'
//import Home from '../views/Home.vue'
import Login from '../views/Auth/Login.vue'
import PasswordForgetting from '../views/PasswordForgetting.vue'

import CreateUser from '../views/Employee/CreateUser.vue'
import PayIn from '../views/Employee/PayIn.vue'
import remittance from '../views/User/remittance.vue'
import History from '../views/Employee/History.vue'
import VeeValidate from "vee-validate";


import { BootstrapVueIcons } from 'bootstrap-vue'

Vue.use(BootstrapVueIcons)


Vue.use(VueRouter)

Vue.use(VeeValidate, {
  inject: true,
  fieldsBagName: "veeFields",
  errorBagName: "veeErrors"
});


//axios.defaults.baseURL = "https://localhost:44396/api/";
axios.defaults.baseURL = "http://ddpbank.somee.com/api/";

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login,
    meta: { layout: LoginLayout }
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
    meta: {
      layout: EmployeeLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.employee]
    }
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
  },
  {
    path: '/employee/histories',
    name: 'History',
    component: History,
    meta: { layout: EmployeeLayout }
  },
  {
    path: '/password/forget',
    name: 'PasswordForgetting',
    component: PasswordForgetting,
    meta: { layout: LoginLayout }
  },
  {
    path: '/user/',
    name: 'UserHome',
    component: UserHome,
    meta: {
      layout: UserLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.user]
    }
  },
  {
    path: '/user/accounts',
    name: 'UserAccounts',
    component: UserAccounts,
    meta: {
      layout: UserLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.user]
    }
  },
  {
    path: '/user/contacts',
    name: 'UserContacts',
    component: UserContacts,
    meta: {
      layout: UserLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.user]
    }
  },
  {
    path: '/user/histories',
    name: 'UserHistories',
    component: UserHistories,
    meta: {
      layout: UserLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.user]
    }
  },
  {
    path: '/user/deptreminders',
    name: 'UserDeptreminders',
    component: UserDeptreminders,
    meta: {
      layout: UserLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.user]
    }
  },
  {
    path: '/admin/',
    name: 'AdminHome',
    component: AdminHome,
    meta: {
      layout: AdminLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.admin]
    }
  },
  {
    path: '/admin/ListEmployee/',
    name: 'ListEmployee',
    component: ListEmployee,
    meta: {
        layout: AdminLayout,
        requiresAuthen: true,
        requiresRole: [constant.role.admin]
    }
},
{
  path: '/admin/DoisoatIN/',
  name: 'in',
  component: DoisoatIN,
  meta: {
      layout: AdminLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.admin]
  }
},
{
  path: '/admin/DoisoatOUT/',
  name: 'out',
  component: DoisoatOUT,
  meta: {
      layout: AdminLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.admin]
  }
},
{
  path: '/admin/addstaff/',
  name: 'AddStaff',
  component: AddStaff,
  meta: { layout: AddStaffLayout,
      requiresAuthen: true,
      requiresRole: [constant.role.admin] }
},

  {
    path: '/user/remittance',
    name: 'remittance',
    component: remittance,
    meta: 
      { 
        layout: UserLayout,
        requiresAuthen: true,
        requiresRole: [constant.role.user] 
      }
  },
]

const router = new VueRouter({
  routes,
  mode: 'history'
})

export default router
