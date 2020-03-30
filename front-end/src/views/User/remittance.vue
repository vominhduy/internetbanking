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
            <label>Thông tin tài khoản</label>
            <b-form-group
              label-cols-sm="12"
              label-cols-md="4"
              label="Tên người dùng"
              label-for="Name"
            >
              <b-form-input
                id="Name"
                name="Name"
                disabled
                v-model="user_detail.name"
                aria-describedby="NameFeedback"
              ></b-form-input>
            </b-form-group>
            <b-form-group
              label-cols-sm="12"
              label-cols-md="4"
              label="Số tài khoản"
              label-for="Name"
            >
              <b-form-input
                id="Name"
                name="Name"
                disabled
                v-model="user_detail.account_number"
                aria-describedby="NameFeedback"
              ></b-form-input>
            </b-form-group>
            <b-form-group label-cols-sm="12" label-cols-md="4" label="Số dư" label-for="Name">
              <b-form-input
                id="Name"
                name="Name"
                disabled
                v-model="user_detail.checking_account.account_balance"
                aria-describedby="NameFeedback"
              ></b-form-input>
            </b-form-group>

            <b-row>
              <b-col>
                <b-button variant="success" size="sm" class="float-right">
                  Đổi tài khoản
                  <b-icon icon="chevron-right"></b-icon>
                </b-button>
              </b-col>
            </b-row>
          </div>
          <label>Thông tin người nhận</label>

          <div class="info_transfer">
            <b-card no-body>
              <b-tabs pills card content-class="mt-3" fill>
                <b-tab title="Trong DDPBank" active>
                  <b-form @submit.stop.prevent="onSubmit">
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tài khoản"
                      label-for="Name"
                    >
                      <b-input-group>
                        <b-form-input
                          id="Name"
                          name="Name"
                          v-validate="{required:true}"
                          :state="validateState('Name')"
                          aria-describedby="NameFeedback"
                          v-model="internal_transfer.from_account"
                        ></b-form-input>
                        <b-input-group-append>
                          <b-dropdown text="Chọn người nhận">
                            <b-dropdown-item
                              v-for="option in user_detail.payess"
                              :key="option.id"
                              :value="option.id"
                              @click="change_payes(option.mnemonic_name)"
                            >{{option.mnemonic_name}}</b-dropdown-item>
                          </b-dropdown>
                        </b-input-group-append>
                        <b-form-invalid-feedback id="NameFeedback">Số tài khoản không được để trống!</b-form-invalid-feedback>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Tên người nhận"
                      label-for="Address"
                    >
                      <b-form-input
                        id="Address"
                        name="Address"
                        v-validate="{required:true}"
                        :state="validateState('Address')"
                        aria-describedby="AddressFeedback"
                        v-model="internal_transfer.mnemonic_name"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="AddressFeedback"
                      >Tên người nhận không được để trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tiền"
                      label-for="Email"
                    >
                      <b-form-input
                        id="Email"
                        :type="'number'"
                        name="Email"
                        v-validate="'required'"
                        :state="validateState('Email')"
                        aria-describedby="EmailFeedback"
                        v-model="internal_transfer.amount"
                      ></b-form-input>
                      <b-form-invalid-feedback id="EmailFeedback">Số tiền không được bỏ trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Nội dung"
                      label-for="Phone"
                    >
                      <b-form-input
                        id="Phone"
                        name="Phone"
                        v-validate="{required:true}"
                        :state="validateState('Phone')"
                        aria-describedby="PhoneFeedback"
                        v-model="internal_transfer.description"
                      ></b-form-input>
                      <b-form-invalid-feedback id="PhoneFeedback">Nội dung không được để trống!</b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Hình thức thanh toán phí"
                      label-for="Gender"
                    >
                      <b-form-select
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
                        >
                          Chuyển khoản
                          <b-icon icon="chevron-right"></b-icon>
                        </b-button>
                      </b-col>
                    </b-row>
                  </b-form>
                </b-tab>

                <b-tab title="Liên ngân hàng">
                  <b-form @submit.stop.prevent="changeComponet">
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Chọn ngân hàng"
                      label-for="Gender"
                    >
                      <b-form-select id="Gender" :options="paid_type"></b-form-select>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tài khoản"
                      label-for="Name"
                    >
                      <b-input-group>
                        <b-form-input
                          id="Name"
                          name="Name"
                          v-validate="{required:true}"
                          :state="validateState('Name')"
                          aria-describedby="NameFeedback"
                        ></b-form-input>
                        <b-input-group-append>
                          <b-dropdown text="Chọn người nhận">
                            <!-- <b-dropdown-item
                              v-for="option in "
                              :key="option.value"
                              :value="option.value"
                              @click="user.Gender = option.text"
                            >{{option.text}}</b-dropdown-item>-->
                          </b-dropdown>
                        </b-input-group-append>
                        <b-form-invalid-feedback id="NameFeedback">Số tài khoản không được để trống!</b-form-invalid-feedback>
                      </b-input-group>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Tên người nhận"
                      label-for="Address"
                    >
                      <b-form-input
                        id="Address"
                        name="Address"
                        v-validate="{required:true}"
                        :state="validateState('Address')"
                        aria-describedby="AddressFeedback"
                      ></b-form-input>
                      <b-form-invalid-feedback
                        id="AddressFeedback"
                      >Tên người nhận không được để trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Số tiền"
                      label-for="Email"
                    >
                      <b-form-input
                        id="Email"
                        :type="'number'"
                        name="Email"
                        v-validate="'required'"
                        :state="validateState('Email')"
                        aria-describedby="EmailFeedback"
                      ></b-form-input>
                      <b-form-invalid-feedback id="EmailFeedback">Số tiền không được bỏ trống!</b-form-invalid-feedback>
                    </b-form-group>
                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Nội dung"
                      label-for="Phone"
                    >
                      <b-form-input
                        id="Phone"
                        :type="'number'"
                        name="Phone"
                        v-validate="{required:true}"
                        :state="validateState('Phone')"
                        aria-describedby="PhoneFeedback"
                      ></b-form-input>
                      <b-form-invalid-feedback id="PhoneFeedback">Nội dung không được để trống!</b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group
                      label-cols-sm="12"
                      label-cols-md="4"
                      label="Hình thức thanh toán phí"
                      label-for="Gender"
                    >
                      <b-form-select id="Gender" :options="paid_type"></b-form-select>
                    </b-form-group>
                    <b-row>
                      <b-col>
                        <b-button
                          class="mb-2 float-right"
                          variant="success"
                          @click.prevent="changeComponet"
                        >
                          Chuyển khoản
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

export default {
  name: "remittance",
  components: {
    NavBar,
    Categories
  },
  data() {
    return {
      show: false,
      component: "Categories",
      info_confirm_otp: {
        full_name: "",
        email: "",
        transaction_id:""
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
      paid_type: [
        {
          value: 1,
          text: "Người gửi trả phí"
        },
        {
          value: 2,
          text: "Người nhận trả phí"
        }
      ],
      internal_transfer: {
        from_account: "",
        to_account: "",
        amount: 0,
        description: "",
        paid_type: 1,
        from_name: ""
      }
    };
  },
  mounted() {
    // lấy thông tin chi tiết của user
    var id = "90ebd0ae-1460-4e0f-a7cd-9e9c98c5eed2";
    try {
      helper
        .call_api("users/" + id, "get", "")
        .then(res => {
          console.log("s");
          console.log(res);
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
              res.data.Payess.forEach(item => {
                var payes = {
                  account_number: item.AccountNumber,
                  id: item.Id,
                  linking_bank_id: item.LinkingBankId,
                  mnemonic_name: item.Mnemonicname
                };
                this.user_detail.payess.push(payes);
              });
            } else {
              // test
              var payes = {
                account_number: "12345689",
                id: "12345678765432",
                linking_bank_id: "1111111111111111",
                mnemonic_name: "item.Mnemonicname"
              };
              this.user_detail.payess.push(payes);
            }
          }
        })
        .catch(err => {
          console.log("e");
          console.log(err);
        });
    } catch {
      console.log("error");
    }
  },
  methods: {
    change_payes(name) {
      this.internal_transfer.mnemonic_name = name;
      this.user_detail.payess.forEach(item => {
        if (item.mnemonic_name === name) {
          this.internal_transfer.from_account = item.account_number;
        }
      });
    },
    changeComponet() {
    // chuyển khoản
        var obj = {
          SourceAccountNumber: this.internal_transfer.from_account,
          DestinationAccountNumber: this.internal_transfer.to_account,
          Money: this.internal_transfer.amount,
          IsSenderPay: this.paid_type == 1 ? true : false,
          Description: this.internal_transfer.description
        }
        helper
        .call_api("users/internaltransfer", "post", obj)
        .then(s => {
          console.log("s");
          console.log(s);
        })
        .catch(err => {
          console.log("e");
          console.log(err);
          this.show = true;
          this.component = "NavBar";
          console.log(this.component);
        });

      this.show = true;
      console.log(this.component);
      this.component = "NavBar";
      this.info_confirm_otp.full_name = this.user_detail.name
      this.info_confirm_otp.transaction_id ="123"
      this.info_confirm_otp.email = this.user_detail.email
      console.log("aaa");
      // helper
      //   .call_api("1", "get", "2")
      //   .then(s => {
      //     console.log("s");
      //     console.log(s);
      //   })
      //   .catch(err => {
      //     console.log("e");
      //     console.log(err);
      //   });
    },
    onSubmit(evt) {
      evt.preventDefault();
      // axios
      //   .post(`employees`, this.user)
      //   .then(res => {
      //     this.respone = res.data;
      //     this.$refs["respone"].show();
      //   })
      //   .catch(err => {
      //     this.empty = true;
      //     console.log(err);
      //   });
    },
    canceled() {
      this.user = {};
    },
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