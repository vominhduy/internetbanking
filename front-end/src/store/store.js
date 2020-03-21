import Vue from 'vue'
import Vuex from 'vuex'
import axios from "axios";
import helper from '../helper/helper'

Vue.use(Vuex)

export const store = new Vuex.Store({
    state:{
        username:'',
        userRole: localStorage.getItem('user_role') || null,
        jwt: localStorage.getItem('access_token') || null,
    },
    getters: {
        loggedIn(state){
            return state.jwt !== null;
        }
    },
    mutations:{
        retrieveToken(state, token){
            state.jwt = token;
        },
        retrieveUserInfo(state, userInfo){
            state.userRole = userInfo.role;
            state.username = userInfo.nameid;
        },
        destroyToken(state){
            state.jwt = null;
            state.userRole = null;
            state.username = null; 
        }
    },
    actions:{
        retrieveLogin(context, credentials){
            return new Promise((resolve, reject) => {
                axios
                .post(`accounts/login`,{
                    Username: credentials.username,
                    Password: credentials.password
                })
                .then(res => {
                    let token = res.data.AccessToken;
                    localStorage.setItem('access_token', token); //aware cross-site scripting
                    context.commit('retrieveToken', token);
                    
                    let userInfo = helper.parseJwt(token);
                    localStorage.setItem('user_role', userInfo.role); //aware cross-site scripting
                    context.commit('retrieveUserInfo', userInfo);
                    resolve(userInfo);
                })
                .catch(err => {
                    console.log(err);
                    reject(err);
                })
            })
        },
        destroyToken(context){
            return new Promise((resolve, reject) => {
                axios
                .post(`accounts/logout`)
                .then(res => {
                    localStorage.removeItem('access_token');
                    localStorage.removeItem('user_role');
                    context.commit('destroyToken');
                    resolve(res);
                })
                .catch(err => {
                    localStorage.removeItem('access_token');
                    localStorage.removeItem('user_role');
                    context.commit('destroyToken');
                    console.log(err);
                    reject(err);
                })
            })
        }
    },
})