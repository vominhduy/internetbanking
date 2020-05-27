<template>
    <div>
        <b-list-group> 
            <b-list-group-item>
                <b-button v-b-toggle.collapse-1 variant="primary">Tài khoản thanh toán
                </b-button>
                <b-collapse id="collapse-1" class="mt-2">
                    <b-card>
                         <b-col lg="12">
                            <b-list-group>
                                <b-list-group-item class="d-flex justify-content-between align-items-center">
                                    Tên tài khoản: {{paymentAccount.Name}}
                                </b-list-group-item>
                                <b-list-group-item class="d-flex justify-content-between align-items-center">
                                    Số tài khoản: {{paymentAccount.Id}}
                                </b-list-group-item>
                                <b-list-group-item class="d-flex justify-content-between align-items-center">
                                    Số dư: {{paymentAccount.AccountBalance}} VNĐ
                                </b-list-group-item>
                                <b-list-group-item class="d-flex justify-content-between align-items-center" v-if="paymentAccount.IsClosed">
                                <span class="text-danger">Đã đóng</span>
                                </b-list-group-item>
                                <b-list-group-item class="d-flex justify-content-between align-items-center" v-if="!paymentAccount.IsClosed">
                                <span class="text-success">Đang hoạt động</span>
                                </b-list-group-item>
                            </b-list-group>
                        </b-col>
                        <b-col lg="4" class="pb-2">
                            <b-button @click="CloseBankAccount" :disabled="paymentAccount.IsClosed" size="sm" variant="danger" style="margin-top:10px">Đóng tài khoản</b-button>
                        </b-col>
                    </b-card>
                </b-collapse>
            </b-list-group-item>
            <b-list-group-item>
                <b-button v-b-toggle.collapse-2 variant="primary">Tài khoản tiết kiệm
                            <b-badge variant="success" pill>{{savingAccounts.length}}</b-badge>
                </b-button>
                <b-collapse id="collapse-2" class="mt-2">
                    <b-card>
                        <b-list-group v-for="item in savingAccounts" :key="item.Id">
                            <b-list-group-item class="d-flex justify-content-between align-items-center">
                                Tên tài khoản: {{item.Name}}
                            </b-list-group-item>
                            <b-list-group-item class="d-flex justify-content-between align-items-center">
                                Số tài khoản: {{item.Id}}
                            </b-list-group-item>
                            <b-list-group-item class="d-flex justify-content-between align-items-center">
                                Số dư: {{item.AccountBalance}} VNĐ
                            </b-list-group-item>
                        </b-list-group>
                    </b-card>
                </b-collapse>
            </b-list-group-item>
        </b-list-group>
    </div>
</template>

<script>
import apiHelper from '../../helper/call_api'
import utilsHelper from '../../helper/helper'

export default {
    name:'UserAccounts',
    data(){
        return{
            paymentAccount: {
                Id: '',
                AccountBalance: '',
                Name: '',
                IsClosed: false,
            },
            savingAccounts:[]
        }
    },
    mounted: function(){
        this.getLinkingAccounts();
    },
    methods: {
        getLinkingAccounts(){
            let me = this;
            apiHelper
                .call_api(`Users`, "get", '')
                .then(res => {
                    if(res.data){
                        if(res.data.CheckingAccount){
                            me.paymentAccount.Id = res.data.CheckingAccount.Id;
                            me.paymentAccount.AccountBalance = res.data.CheckingAccount.AccountBalance;
                            me.paymentAccount.Name = res.data.CheckingAccount.Name;
                            me.paymentAccount.IsClosed = res.data.CheckingAccount.IsClosed;
                            localStorage.setItem('IsClosedBank', me.paymentAccount.IsClosed);
                        }

                        if(res.data.SavingsAccounts.length > 0){
                            res.data.SavingsAccounts.forEach(element => {
                                me.savingAccounts.push({
                                    Id: element.Id,
                                    AccountBalance: element.AccountBalance,
                                    Name: element.Name,
                                })
                            })
                        }
                    }
                })
                .catch(err => {
                    console.error(err);
                });
        },
        CloseBankAccount(){
            let me = this;
            apiHelper
                .call_api(`Users/BankAccounts/${me.paymentAccount.Id}/close`, "post", {})
                .then(res => {
                    if(res.data){
                        if(res.status == 204){
                            utilsHelper.showErrorMsg(me, 'Thông tin tài khoản không hợp lệ!');
                            return;
                        }

                        me.paymentAccount.IsClosed = res.data;
                        localStorage.setItem('IsClosedBank', me.paymentAccount.IsClosed);
                    }
                })
                .catch(err => {
                    utilsHelper.showErrorMsg(me, 'Lỗi hệ thống!');
                    console.error(err);
                });
        }
    }
}
</script>