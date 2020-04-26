import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from 'axios'
import AdminLayout from '../layouts/AdminLayout.vue'
import LoginLayout from '../layouts/LoginLayout.vue'
import EmployeeLayout from '../layouts/EmployeeLayout.vue'
import UserLayout from '../layouts/UserLayout.vue'
import constant from '../helper/constant'
import EditRemoveStaffLayout from '../layouts/EditRemoveStaff.vue'
import SearchStaffLayout from '../layouts/SearchStaffLayout.vue'
import AddStaffLayout from '../layouts/AddStaffLayout.vue'

import EmployeeHome from '../views/Employee/Home.vue'

import UserHome from '../views/User/Home.vue'

import AdminHome from '../views/Admin/Home.vue'
import SearchStaff from '../views/Admin/SearchStaff.vue'
import EditRemoveStaff from '../views/Admin/EditRemoveStaff.vue'
import AddStaff from '../views/Admin/AddStaff.vue'

//import Home from '../views/Home.vue'
import Login from '../views/Auth/Login.vue'

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


axios.defaults.baseURL = "https://localhost:44396/api/";
//axios.defaults.baseURL = "http://ddpbank.somee.com/api/";\

const routes = [{
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
        path: '/admin/search/',
        name: 'SearchStaff',
        component: SearchStaff,
        meta: { layout: SearchStaffLayout,
            requiresAuthen: true,
            requiresRole: [constant.role.admin] }
    },
    {
        path: '/admin/edit-remove/',
        name: 'EditRemoveStaff',
        component: EditRemoveStaff,
        meta: { layout: EditRemoveStaffLayout,
            requiresAuthen: true,
            requiresRole: [constant.role.admin] }
    },
    {
        path: '/admin/add-staff/',
        name: 'AddStaff',
        component: AddStaff,
        meta: { layout: AddStaffLayout,
            requiresAuthen: true,
            requiresRole: [constant.role.admin] }
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
        path: '/user/remittance',
        name: 'remittance',
        component: remittance,
        meta: { layout: UserLayout }
    },
    {
        path: '/employee/histories',
        name: 'History',
        component: History,
        meta: { layout: EmployeeLayout }
    }
]

const router = new VueRouter({
    routes,
    mode: 'history'
})

export default router