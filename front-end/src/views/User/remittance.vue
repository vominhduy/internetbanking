<template>
  <div>
    <b-card-group v-if="show==false" class="bcard-shadow">
      <b-card>
        <div class="header">
          <h4 class="title">
            <b-icon icon="chevron-right"></b-icon>&nbsp;Chuyển khoản
          </h4>
          <hr />
        </div>
        <div class="body">
          <div class="info_user">
            <h6>Thông tin tài khoản</h6>
            <div v-show="isLoadingUser == 0">
              <b-spinner
                style="width: 3rem; height: 3rem;"
                variant="primary"
                label="Large Spinner"
                type="grow"
              ></b-spinner>
            </div>
            <div v-show="isLoadingUser == 1">
              <b-form-group
                label-cols-sm="12"
                label-cols-md="4"
                label="Tên người dùng"
                label-for="Name"
              >
                <h6 id="Name" name="Name" aria-describedby="NameFeedback">{{user_detail.name}}</h6>
              </b-form-group>
              <b-form-group
                label-cols-sm="12"
                label-cols-md="4"
                label="Số tài khoản"
                label-for="Name"
              >
                <h6
                  id="Name"
                  name="Name"
                  aria-describedby="NameFeedback"
                >{{user_detail.account_number}}</h6>
              </b-form-group>
              <b-form-group label-cols-sm="12" label-cols-md="4" label="Số dư" label-for="Name">
                <h6
                  id="Name"
                  name="Name"
                  aria-describedby="NameFeedback"
                >{{user_detail.checking_account.account_balance}}</h6>
              </b-form-group>

              <b-row>
                <b-col>
                  <b-button :disabled="IsClosedBankAccount" variant="primary" size="sm" class="float-right">
                    Đổi tài khoản
                    <b-icon icon="chevron-right"></b-icon>
                  </b-button>
                </b-col>
              </b-row>
            </div>
          </div>
          <h6>Thông tin người nhận</h6>

          <div class="info_transfer">
            <b-card no-body>
              <b-tabs :disabled="IsClosedBankAccount"
                pills
                card
                content-class="mt-3"
                fill
                v-model="tabIndex"
                v-on:activate-tab="tabActivated"
              >
                <b-tab title="Trong DDPBank" active id="internal" :disabled="IsClosedBankAccount">
                  <b-form @submit.stop.prevent="onSubmit">
                    <b-form-group :disabled="IsClosedBankAccount"
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tài khoản"
                      label-for="Name"
                    >
                      <b-input-group>
                        <b-form-input
                          id="inPayees"
                          name="inPayees"
                          v-validate="{required:true}"
                          :state="validateState('inPayees')"
                          aria-describedby="inPayeesFeedback"
                          v-model="internal_transfer.to_account"
                          v-on:blur="loadInfoDestination"
                          :disabled="form_internal_validate.isDisable == 1 || IsClosedBankAccount"
                        ></b-form-input>
                        <b-input-group-append>
                          <b-dropdown
                            text="Chọn người nhận"
                            :disabled="form_internal_validate.isDisable == 1 || IsClosedBankAccount"
                          >
                            <b-dropdown-item :disabled="IsClosedBankAccount"
                              v-for="option in payees_filter"
                              :key="option.id"
                              :value="option.id"
                              @click="change_payes(option.mnemonic_name)"
                            >{{option.mnemonic_name}}</b-dropdown-item>
                          </b-dropdown>
                        </b-input-group-append>
                        <b-form-invalid-feedback
                          id="inPayeesFeedback"
                        >Số tài khoản không được để trống!</b-form-invalid-feedback>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group label-cols-sm="12" label-cols-md="4" label>
                      <b-input-group>
                        <b-form-checkbox :disabled="IsClosedBankAccount"
                          id="checkbox-isSaveRecepientInternal"
                          name="checkbox-isSaveRecepientInternal"
                          v-model="isSaveRecepientInternal"
                        >Lưu vào danh bạ</b-form-checkbox>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group :disabled="IsClosedBankAccount"
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Tên người nhận"
                      label-for="inPayeesName"
                    >
                      <div
                        class="spinner-grow text-primary"
                        role="status"
                        v-show="form_internal_validate.isLoading == 1"
                      >
                        <span class="sr-only">Loading...</span>
                      </div>
                      <b-form-input
                        id="inPayeesName"
                        name="inPayeesName"
                        v-validate="{required:true}"
                        :state="validateState('inPayeesName')"
                        aria-describedby="inPayeesNameFeedback"
                        v-model="internal_transfer.to_name"
                        disabled="disabled"
                        v-show="form_internal_validate.isLoading == 0"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="inPayeesNameFeedback"
                      >Tên người nhận không được để trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tiền"
                      label-for="inAmount"
                    >
                      <b-form-input :disabled="IsClosedBankAccount"
                        id="inAmount"
                        :type="'number'"
                        name="inAmount"
                        min="1000"
                        step="1000"
                        v-validate="'required'"
                        :state="validateState('inAmount')"
                        aria-describedby="inAmountFeedback"
                        v-model="internal_transfer.amount"
                      ></b-form-input>
                      <b-form-invalid-feedback id="inAmountFeedback">Số tiền không được bỏ trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Nội dung"
                      label-for="inDescription"
                    >
                      <b-form-input :disabled="IsClosedBankAccount"
                        id="inDescription"
                        name="inDescription"
                        v-validate="{required:true}"
                        :state="validateState('inDescription')"
                        aria-describedby="inDescriptionFeedback"
                        v-model="internal_transfer.description"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="inDescriptionFeedback"
                      >Nội dung không được để trống!</b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Hình thức thanh toán phí"
                      label-for="Gender"
                    >
                      <b-form-select :disabled="IsClosedBankAccount"
                        id="Gender"
                        v-model="internal_transfer.paid_type"
                        :options="paid_type"
                      ></b-form-select>
                    </b-form-group>
                    <b-row>
                      <b-col>
                        <b-button
                          class="mb-2 float-right"
                          variant="success"
                          @click.prevent="changeComponet"
                          :disabled="next_step == 1 ||  IsClosedBankAccount"
                        >
                          Tiếp tục
                          <b-icon icon="chevron-right"></b-icon>
                        </b-button>
                      </b-col>
                    </b-row>
                  </b-form>
                </b-tab>

                <b-tab title="Liên ngân hàng" id="external" :disabled="IsClosedBankAccount">
                  <b-form @submit.stop.prevent="changeComponet">
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Chọn ngân hàng"
                      label-for="linkbank"
                    >
                      <div
                        class="spinner-grow text-primary"
                        role="status"
                        v-show="form_external_validate.isLoadingBanking == 0"
                      >
                        <span class="sr-only">Loading...</span>
                      </div>

                      <b-form-select :disabled="IsClosedBankAccount"
                        id="linkbank"
                        :options="external_bank"
                        @change="change_banking"
                        v-show="form_external_validate.isLoadingBanking == 1"
                      ></b-form-select>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tài khoản"
                      label-for="exPayeesAcc"
                    >
                      <b-input-group>
                        <b-form-input
                          id="exPayeesAcc"
                          name="exPayeesAcc"
                          v-validate="{required:true}"
                          :state="validateState('exPayeesAcc')"
                          aria-describedby="exPayeesAccFeedback"
                          v-model="external_transfer.to_account"
                          v-on:blur="loadInfoDestinationExternal"
                          :disabled="form_external_validate.isDisable == 1 || IsClosedBankAccount"
                        ></b-form-input>
                        <b-input-group-append>
                          <b-dropdown
                            text="Chọn người nhận"
                            :disabled="form_external_validate.isDisable == 1 || IsClosedBankAccount"
                          >
                            <b-dropdown-item :disabled="IsClosedBankAccount"
                              v-for="option in payees_filter"
                              :key="option.id"
                              :value="option.id"
                              @click="change_payes_external(option.mnemonic_name)"
                            >{{option.mnemonic_name}}</b-dropdown-item>
                          </b-dropdown>
                        </b-input-group-append>
                        <b-form-invalid-feedback
                          id="exPayeesAccFeedback"
                        >Số tài khoản không được để trống!</b-form-invalid-feedback>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group label-cols-sm="12" label-cols-md="4" label>
                      <b-input-group>
                        <b-form-checkbox :disabled="IsClosedBankAccount"
                          id="checkbox-isSaveRecepientExternal"
                          name="checkbox-isSaveRecepientExternal"
                          v-model="isSaveRecepientExternal"
                        >Lưu vào danh bạ</b-form-checkbox>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Tên người nhận"
                      label-for="exPayeesName"
                    >
                      <div
                        class="spinner-grow text-primary"
                        role="status"
                        v-show="form_external_validate.isLoading == 1"
                      >
                        <span class="sr-only">Loading...</span>
                      </div>
                      <b-form-input
                        id="exPayeesName"
                        name="exPayeesName"
                        v-validate="{required:true}"
                        :state="validateState('exPayeesName')"
                        aria-describedby="exPayeesNameFeedback"
                        v-model="external_transfer.to_name"
                        disabled="disabled"
                        v-show="form_external_validate.isLoading == 0"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="exPayeesNameFeedback"
                      >Tên người nhận không được để trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tiền"
                      label-for="exAmount"
                    >
                      <b-form-input :disabled="IsClosedBankAccount"
                        id="exAmount"
                        :type="'number'"
                        name="exAmount"
                        min="1000"
                        step="1000"
                        v-validate="'required'"
                        :state="validateState('exAmount')"
                        aria-describedby="exAmountFeedback"
                        v-model="external_transfer.amount"
                      ></b-form-input>
                      <b-form-invalid-feedback id="exAmountFeedback">Số tiền không được bỏ trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Nội dung"
                      label-for="exDescription"
                    >
                      <b-form-input :disabled="IsClosedBankAccount"
                        id="exDescription"
                        name="exDescription"
                        v-validate="{required:true}"
                        :state="validateState('exDescription')"
                        aria-describedby="exDescriptionFeedback"
                        v-model="external_transfer.description"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="exDescriptionFeedback"
                      >Nội dung không được để trống!</b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Hình thức thanh toán phí"
                      label-for="Gender"
                    >
                      <b-form-select :disabled="IsClosedBankAccount"
                        id="Gender"
                        v-model="external_transfer.paid_type"
                        :options="paid_type"
                      ></b-form-select>
                    </b-form-group>
                    <b-row>
                      <b-col>
                        <b-button
                          class="mb-2 float-right"
                          variant="success"
                          @click.prevent="changeComponet"
                          :disabled="next_step == 1 ||  IsClosedBankAccount"
                        >
                          Tiếp tục
                          <b-icon icon="chevron-right"></b-icon>
                        </b-button>
                      </b-col>
                    </b-row>
                  </b-form>
                </b-tab>
              </b-tabs>
            </b-card>
          </div>
        </div>
      </b-card>
    </b-card-group>

    <component v-if="show==true" v-bind:is="component" v-bind:info_confirm_otp="info_confirm_otp" />
  </div>
</template>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>

<script>
import NavBar from "@/components/User/confirm_otp.vue";
import Categories from "@/components/User/transfer_money_info.vue";
import axios from "axios";
import helper from "../../helper/call_api.js";
import utilsHelper from "../../helper/helper";

export default {
  name: "remittance",
  components: {
    NavBar,
    Categories
  },
  data() {
    return {
      isLoadingUser: 0,
      show: false,
      next_step: 1,
      my_bank_id: "d7b6f37e-0a82-4894-84c1-91a3e89f6fed",
      tabIndex: 0,
      component: "Categories",
      info_confirm_otp: {
        full_name: "",
        email: "",
        transaction_id: "",
        destination_account_number: "",
        amount: 0
      },
      user_detail: {
        account_number: "",
        checking_account: {
          id: "",
          description: "",
          account_balance: 0,
          name: ""
        },
        payess: [],
        name: "",
        email: ""
      },
      payees_filter: [],
      paid_type: [
        {
          value: "1",
          text: "Người gửi trả phí"
        },
        {
          value: "2",
          text: "Người nhận trả phí"
        }
      ],
      external_bank: [],
      internal_transfer: {
        from_account: "",
        to_account: "",
        amount: 0,
        description: "",
        paid_type: "1",
        to_name: ""
      },
      external_transfer: {
        from_account: "",
        to_account: "",
        amount: 0,
        description: "",
        paid_type: "1",
        to_name: "",
        destination_linking_bank_id: ""
      },
      isSaveRecepientInternal: false,
      isSaveRecepientExternal: false,
      form_internal_validate: {
        isLoading: 0,
        isDisable: 0
      },
      form_external_validate: {
        isLoading: 0,
        isDisable: 0,
        isLoadingBanking: 0
      },
      IsClosedBankAccount: localStorage.getItem('IsClosedBank') == 'true'
    };
  },
  mounted() {
    this.isLoadingUser = 0;
    // lấy thông tin chi tiết của user
    try {
      helper
        .call_api("users", "get", "")
        .then(res => {
          if (res.status == "200") {
            this.user_detail.name = res.data.Name;
            this.user_detail.account_number = res.data.AccountNumber;
            this.user_detail.email = res.data.Email;
            this.user_detail.checking_account.id = res.data.CheckingAccount.Id;
            this.user_detail.checking_account.description =
              res.data.CheckingAccount.Description;
            this.user_detail.checking_account.account_balance =
              res.data.CheckingAccount.AccountBalance;
            this.user_detail.checking_account.name =
              res.data.CheckingAccount.Name;
            if (res.data.Payees.length > 0) {
              res.data.Payees.forEach(item => {
                var payes = {
                  account_number: item.AccountNumber,
                  id: item.Id,
                  linking_bank_id: item.LinkingBankId,
                  mnemonic_name: item.MnemonicName
                };
                this.user_detail.payess.push(payes);
              });
            }

            // init tab
            // Load ds người nhận cùng ngân hàng
            this.user_detail.payess.forEach(item => {
              if (item.linking_bank_id == this.my_bank_id) {
                this.payees_filter.push(item);
              }
            });

            this.next_step = 0;
            this.isLoadingUser = 1;
          }
        })
        .catch(err => {
          utilsHelper.showErrorMsg(this, "Lỗi lấy thông tin tài khoản.");
          this.isLoadingUser = 1;
        });
    } catch {
      utilsHelper.showErrorMsg(this, "Lỗi lấy thông tin tài khoản.");
      this.isLoadingUser = 1;
    }
  },
  methods: {
    change_payes(name) {
      this.internal_transfer.to_name = name;
      this.user_detail.payess.forEach(item => {
        if (item.mnemonic_name === name) {
          this.internal_transfer.to_account = item.account_number;
        }
      });
    },
    changeComponet(evt) {
      this.next_step = 1;
      if (this.tabIndex == 0) {
        // validate
        Promise.all([
          this.$validator.validate("inDescription"),
          this.$validator.validate("inAmount"),
          this.$validator.validate("inPayeesName"),
          this.$validator.validate("inPayees")
        ]).then(result => {
          if (!result.every((val, i, arr) => val == true)) {
            this.next_step = 0;
            return;
          }

          // chuyển khoản nội bộ
          var obj = {
            SourceAccountNumber: this.internal_transfer.from_account,
            DestinationAccountNumber: this.internal_transfer.to_account,
            Money: this.internal_transfer.amount,
            IsSenderPay: this.paid_type == "1" ? true : false,
            Description: this.internal_transfer.description,
            DestinationLinkingBankId: this.my_bank_id,
            IsSaveRecepient: this.isSaveRecepientInternal
          };
          helper
            .call_api("users/internaltransfer", "post", obj)
            .then(res => {
              if (res.status == "200") {
                this.info_confirm_otp.full_name = this.user_detail.name;
                this.info_confirm_otp.transaction_id = res.data;
                this.info_confirm_otp.email = this.user_detail.email;
                this.info_confirm_otp.destination_account_number = this.internal_transfer.to_account;
                this.info_confirm_otp.amount = this.internal_transfer.amount;

                this.next_step = 0;
                this.show = true;
                this.component = "NavBar";
              } else {
                console.log(res);
                this.next_step = 0;
              }
            })
            .catch(err => {
              console.log(err);
              this.next_step = 0;
              utilsHelper.showErrorMsg(this, "Lỗi chuyển khoản.");
            });
        });
      } else if (this.tabIndex == 1) {
        // validate
        Promise.all([
          this.$validator.validate("exPayeesAcc"),
          this.$validator.validate("exPayeesName"),
          this.$validator.validate("exAmount"),
          this.$validator.validate("exDescription")
        ]).then(result => {
          if (!result.every((val, i, arr) => val == true)) {
            this.next_step = 0;
            return;
          }

          // liên ngân hàng
          var obj2 = {
            SourceAccountNumber: this.external_transfer.from_account,
            DestinationAccountNumber: this.external_transfer.to_account,
            Money: this.external_transfer.amount,
            IsSenderPay: this.external_transfer == 1 ? true : false,
            Description: this.external_transfer.description,
            DestinationLinkingBankId: this.external_transfer
              .destination_linking_bank_id,
            IsSaveRecepient: this.isSaveRecepientExternal
          };
          helper
            .call_api("users/externaltransfer", "post", obj2)
            .then(res => {
              if (res.status == "200") {
                this.info_confirm_otp.full_name = this.user_detail.name;
                this.info_confirm_otp.transaction_id = res.data;
                this.info_confirm_otp.email = this.user_detail.email;
                this.info_confirm_otp.destination_account_number = this.external_transfer.to_account;
                this.info_confirm_otp.amount = this.external_transfer.amount;

                this.next_step = 0;
                this.show = true;
                this.component = "NavBar";
              } else {
                utilsHelper.showErrorMsg(this, "Lỗi chuyển khoản.");
                this.next_step = 0;
              }
            })
            .catch(err => {
              this.next_step = 0;
              utilsHelper.showErrorMsg(this, "Lỗi chuyển khoản.");
            });
        });
      }
    },
    onSubmit(evt) {
      evt.preventDefault();
    },

    tabActivated(newTabIndex, oldTabIndex) {
      this.form_external_validate.isDisable = 1;

      this.next_step = 1;
      this.payees_filter = [];
      //this.external_bank = [];
      this.tabIndex = newTabIndex;
      if (newTabIndex == 1) {
        // lay danh sach ngan hang
        if (this.external_bank.length == 0) {
          helper
            .call_api("LinkingBank/LinkingBanks", "get", "")
            .then(res => {
              res.data.forEach(item => {
                var bank = {
                  value: item.Id,
                  text: item.Name
                };
                if (item.Id != this.my_bank_id) {
                  this.external_bank.push(bank);
                }
              });

              this.form_external_validate.isDisable = 0;
              this.form_external_validate.isLoadingBanking = 1;
            })
            .catch(err => {
              this.empty = true;
              utilsHelper.showErrorMsg(this, "Lỗi lấy danh sách ngân hàng.");

              this.form_external_validate.isDisable = 0;
              this.form_external_validate.isLoadingBanking = 1;
            });
        } else {
          this.form_external_validate.isDisable = 0;
          this.form_external_validate.isLoadingBanking = 1;
        }

        // clear
        //this.external_transfer.from_account = "";
        this.external_transfer.to_account = "";
        this.external_transfer.amount = 0;
        this.external_transfer.description = "";
        this.external_transfer.paid_type = 1;
        this.external_transfer.to_name = "";
        //this.external_transfer.destination_linking_bank_id = "";
        this.next_step = 0;
      } else {
        // Load ds người nhận cùng ngân hàng
        this.user_detail.payess.forEach(item => {
          if (item.linking_bank_id == this.my_bank_id) {
            this.payees_filter.push(item);
          }
        });

        //clear data
        // this.internal_transfer.from_account = "";
        this.internal_transfer.to_account = "";
        this.internal_transfer.amount = 0;
        this.internal_transfer.description = "";
        this.internal_transfer.paid_type = 1;
        this.internal_transfer.to_name = "";
        this.next_step = 0;
      }
    },
    change_banking(value) {
      this.payees_filter = [];
      this.external_transfer.destination_linking_bank_id = value;
      this.external_transfer.to_account = "";
      this.external_transfer.to_name = "";
      // Load ds người nhận cùng ngân hàng
      this.user_detail.payess.forEach(item => {
        if (
          item.linking_bank_id != this.my_bank_id &&
          item.linking_bank_id == value
        ) {
          this.payees_filter.push(item);
        }
      });
    },
    change_payes_external(name) {
      this.external_transfer.to_name = name;
      this.payees_filter.forEach(item => {
        if (item.mnemonic_name === name) {
          this.external_transfer.to_account = item.account_number;
        }
      });
    },
    loadInfoDestination() {
      if (this.internal_transfer.to_account == "") {
        return;
      }
      this.form_internal_validate.isDisable = 1;
      this.form_internal_validate.isLoading = 1;
      helper
        .call_api(
          "users/infotransfer?accountNumber=" +
            this.internal_transfer.to_account,
          "get",
          ""
        )
        .then(res => {
          if (res.status == "200") {
            this.internal_transfer.to_name = res.data.Name;
          } else {
            this.internal_transfer.to_name = "";
            utilsHelper.showErrorMsg(this, "Tài khoản không tồn tại.");
          }
          this.form_internal_validate.isDisable = 0;
          this.form_internal_validate.isLoading = 0;
        })
        .catch(err => {
          utilsHelper.showErrorMsg(this, "Tài khoản không tồn tại.");
          this.internal_transfer.to_name = "";

          this.form_internal_validate.isDisable = 0;
          this.form_internal_validate.isLoading = 0;
        });
    },
    loadInfoDestinationExternal() {
      if (this.external_transfer.to_account == "") {
        return;
      }
      this.form_external_validate.isDisable = 1;
      this.form_external_validate.isLoading = 1;

      helper
        .call_api(
          "users/infotransfer?accountNumber=" +
            this.external_transfer.to_account +
            "&bankId=" +
            this.external_transfer.destination_linking_bank_id,
          "get",
          ""
        )
        .then(res => {
          if (res.status == "200") {
            this.external_transfer.to_name = res.data.Name;
          } else {
            utilsHelper.showErrorMsg(this, "Tài khoản không tồn tại.");
            this.external_transfer.to_name = "";
          }

          this.form_external_validate.isDisable = 0;
          this.form_external_validate.isLoading = 0;
        })
        .catch(err => {
          utilsHelper.showErrorMsg(this, "Tài khoản không tồn tại.");
          this.external_transfer.to_name = "";

          this.form_external_validate.isDisable = 0;
          this.form_external_validate.isLoading = 0;
        });
    },
    canceled() {},
    validateState(ref) {
      if (
        this.veeFields[ref] &&
        (this.veeFields[ref].dirty || this.veeFields[ref].validated)
      ) {
        return !this.veeErrors.has(ref);
      }
      return null;
    }
  }
};
</script>