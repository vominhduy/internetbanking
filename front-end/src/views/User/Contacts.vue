<template>
    <div>
        <h1 style="float:left">Danh bạ người nhận</h1>
        <b-button variant="success" style="margin:10px" @click="onAdd">
            <b-icon icon="plus"></b-icon>
        </b-button>
        <b-table striped hover :items="items" :fields="fields">
             <template v-slot:cell(action)="row">
                <b-button size="sm" @click="onEdit(row)" class="mr-2">
                    Sửa
                </b-button>
                <b-button size="sm" @click="onDelete(row)" class="mr-2">
                    Xóa
                </b-button>
            </template>
        </b-table>

        <b-modal id="modal-1" title="BootstrapVue" ref="my-modal">
            <template v-slot:modal-footer>
                <div class="w-100">
                </div>
            </template>
            <b-form @submit.stop.prevent="onSubmit" v-if="show">
                <b-form-group label-cols-sm="12" label-cols-md="4" label="Ngân hàng" label-for="linkingBank">
                    <b-form-select id="linkingBank" v-model="linkingBank" :options="linkingBankList"
                        v-bind:disabled="contacInfo.IdContact != ''"
                        v-validate="{required:true}"
                        :state="validateState('linkingBank')"
                        aria-describedby="linkingBankFeedback"></b-form-select>
                    <b-form-invalid-feedback
                        id="linkingBankFeedback"
                        >Không được để trống!</b-form-invalid-feedback>
                </b-form-group>
                <b-form-group
                    label-cols-sm="12"
                    label-cols-md="4"
                    label="Số tài khoản"
                    label-for="accountNumber">
                    <b-form-input
                        id="accountNumber"
                        name="accountNumber"
                        v-bind:disabled="contacInfo.IdContact != ''"
                        v-model="contacInfo.AccountNumber"
                        v-validate="{required:true}"
                        :state="validateState('accountNumber')"
                        aria-describedby="accountNumberFeedback">
                    </b-form-input>
                    <b-form-invalid-feedback
                        id="accountNumberFeedback"
                        >Số tài khoản không được để trống!</b-form-invalid-feedback>
                </b-form-group>
                <b-form-group
                    label-cols-sm="12"
                    label-cols-md="4"
                    label="Tên gợi nhớ"
                    label-for="mnemonicName">
                    <b-form-input
                        id="mnemonicName"
                        name="mnemonicName"
                        v-model="contacInfo.MnemonicName">
                    </b-form-input>
                </b-form-group>
                <b-form-group>
                    <b-row>
                    <b-col>
                        <b-button block type="submit" variant="success">Lưu danh bạ</b-button>
                    </b-col>
                    <b-col>
                        <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
                    </b-col>
                    </b-row>
                </b-form-group>
            </b-form>
        </b-modal>
    </div>
</template>

<script>
import apiHelper from '../../helper/call_api'

export default {
    name:'UserContacts',
    data() {
        return {
            // Note `isActive` is left out and will not appear in the rendered table
            fields: ['Account_Number', 'Bank', 'Mnemonic_Name', 'action'],
            items: [],
            contacInfo: {
                IdContact: '',
                AccountNumber:'',
                LinkingBankId: '',
                MnemonicName:''
            },
            linkingBank: '',
            linkingBankList:[],
            show: true,
        }
    },
    mounted: function() {
        this.getLinkingBankInfo();
        this.getAllContacts();
    },
    methods:{
        onSubmit(evt) {
            let me = this;
            evt.preventDefault();
            this.$validator.validateAll().then(result => {
                if (!result) {
                    return;
                }
                me.contacInfo.LinkingBankId = me.linkingBank;
                if(!me.contacInfo.IdContact){
                    apiHelper
                    .call_api(`Users/Payees`, "post", me.contacInfo)
                    .then(res => {
                        if(res.status == 204){
                            me.showErrorMsg('Thông tin tài khoản không hợp lệ!');
                            return;
                        }
                        if(res.data){
                            let isActive = me.items.length > 0 ? !me.items[me.items.length - 1].isActive : true;
                            let linkKingBank = me.linkingBankList.filter(x => x.value == res.data.LinkingBankId);
                            me.items.push({
                                isActive: isActive,
                                Account_Number: res.data.AccountNumber,
                                Bank: linkKingBank[0].text,
                                LinkingBankId: linkKingBank[0].value,
                                Mnemonic_Name: res.data.MnemonicName,
                                Id: res.data.Id
                            })
                        }
                        me.showSuccessfullMsg('Dữ liệu được thêm thành công');
                        me.canceled();
                    })
                    .catch(err => {
                        console.error(err);
                    });
                }
                else 
                {
                    apiHelper
                    .call_api(`Users/Payees/${me.contacInfo.IdContact}`, "put",me.contacInfo)
                    .then(res => {
                        if(res){
                            let udpatedContact = me.items.filter(x => x.Id === me.contacInfo.IdContact);
                            if(udpatedContact.length > 0){
                                udpatedContact[0].Mnemonic_Name = me.contacInfo.MnemonicName;
                            }
                            me.showSuccessfullMsg('Dữ liệu được cập nhật thành công');
                        }
                    })
                    .catch(err => {
                        console.error(err);
                    });
                }
                
            });
        },
        getLinkingBankInfo() {
            let me = this;
            apiHelper
                .call_api(`LinkingBank/LinkingBanks`, "get", '')
                .then(res => {
                    if(res.data.length > 0) {
                        me.linkingBank = res.data[0].Id;
                        res.data.forEach(function(item){
                            me.linkingBankList.push({
                                value: item.Id,
                                text: item.Name
                            })
                        });
                    }
                })
                .catch(err => {
                    console.error(err);
                });
        },
        getAllContacts(){
            let me = this;
            apiHelper
                .call_api(`Users`, "get", '')
                .then(res => {
                    if(res.data.Payees.length > 0){
                        let isActive = true;
                        res.data.Payees.forEach(element => {
                            let linkKingBank = me.linkingBankList.filter(x => x.value == element.LinkingBankId);
                            if(linkKingBank.length > 0) {
                                me.items.push({
                                    isActive: isActive,
                                    Account_Number: element.AccountNumber,
                                    Bank: linkKingBank[0].text,
                                    LinkingBankId: linkKingBank[0].value,
                                    Mnemonic_Name: element.MnemonicName,
                                    Id: element.Id
                                })
                                isActive = !isActive;
                            }
                        });
                    }
                })
                .catch(err => {
                    console.error(err);
                });
        },
        validateState(ref) {
            if (
                this.veeFields[ref] &&
                (this.veeFields[ref].dirty || this.veeFields[ref].validated)
            ) {
                return !this.veeErrors.has(ref);
            }
            return null;
        },
        onAdd(){
            if(this.contacInfo.IdContact){
                this.clearFormData();
            }
            this.$refs['my-modal'].show();
        },
        onEdit(row){
            this.contacInfo.AccountNumber = row.item.Account_Number;
            this.contacInfo.LinkingBankId = row.item.LinkingBankId;
            this.contacInfo.MnemonicName = row.item.Mnemonic_Name;
            this.contacInfo.IdContact = row.item.Id;
            this.linkingBank = row.item.LinkingBankId;
            this.$refs['my-modal'].show();
        },
        onDelete(row){
            let me = this;
            this.$bvModal.msgBoxConfirm('Xóa danh bạ này?', {
                title: 'Xác nhận',
                size: 'sm',
                buttonSize: 'sm',
                okVariant: 'danger',
                okTitle: 'YES',
                cancelTitle: 'NO',
                footerClass: 'p-2',
                hideHeaderClose: false,
                centered: true
            })
            .then(value => {
                if(value){
                    apiHelper
                    .call_api(`Users/Payees/${row.item.Id}`, "get",'')
                    .then(res => {
                        if(res){
                            let deletedContact = me.items.filter(x => x.Id === me.contacInfo.IdContact);
                            if(deletedContact.length > 0){
                                me.items.splice( me.items.indexOf(deletedContact[0]), 1 );
                            }
                            me.showSuccessfullMsg('Dữ liệu đã được xóa!');
                        }
                    })
                    .catch(err => {
                        console.error(err);
                    });
                }
            })
            .catch(err => {
                console.error(err);
            })
        },
        clearFormData(){
            this.linkingBank = this.linkingBankList[0].value;
            this.contacInfo.IdContact = '';
            this.contacInfo.AccountNumber = '';
            this.contacInfo.LinkingBankId = this.linkingBank;
            this.contacInfo.MnemonicName = '';
            
            // Trick to reset/clear native browser form validation state
            this.show = false
            this.$nextTick(() => {
                this.show = true
            })
        },
        canceled(){
            this.clearFormData();
            this.$refs['my-modal'].hide();
        },
        showSuccessfullMsg(msg) {
            this.$bvModal.msgBoxOk(msg, {
                title: 'Thông báo',
                size: 'sm',
                buttonSize: 'sm',
                okVariant: 'success',
                headerClass: 'p-2 border-bottom-0',
                footerClass: 'p-2 border-top-0',
                centered: true
            });
        },
        showErrorMsg(msg) {
            this.$bvModal.msgBoxOk(msg, {
                title: 'Thông báo',
                size: 'sm',
                buttonSize: 'sm',
                okVariant: 'danger',
                headerClass: 'p-2 border-bottom-0',
                footerClass: 'p-2 border-top-0',
                centered: true
            });
        },
    }
}
</script>