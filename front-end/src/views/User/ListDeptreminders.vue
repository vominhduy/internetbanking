<template>
  <div>
    <b-list-group>
      <b-list-group-item>
        <b-button v-b-toggle.collapse-1 variant="primary">
          Danh sách nợ đã gửi
          <b-badge variant="success" pill>{{sentDeptReminders.length}}</b-badge>
        </b-button>
        <b-collapse id="collapse-1" class="mt-2">
          <b-card>
            <b-table striped hover :items="sentDeptReminders" :fields="fields">
              <template v-slot:cell(action)="row">
                <b-button size="sm" @click="onCancel(row)" class="mr-2">Hủy</b-button>
              </template>
            </b-table>
          </b-card>
        </b-collapse>
      </b-list-group-item>
      <b-list-group-item>
        <b-button v-b-toggle.collapse-2 variant="primary">
          Danh sách nợ đã nhận
          <b-badge variant="success" pill>{{receivedDeptReminders.length}}</b-badge>
        </b-button>
        <b-collapse id="collapse-2" class="mt-2">
          <b-card>
            <b-table striped hover :items="receivedDeptReminders" :fields="fields">
              <template v-slot:cell(action)="row">
                <b-button size="sm" @click="onResolve(row)" class="btn btn-success">Thanh toán</b-button>
                <b-button size="sm" @click="onCancel(row)" class="mr-2">Hủy</b-button>
              </template>
            </b-table>
          </b-card>
        </b-collapse>
      </b-list-group-item>
    </b-list-group>
    <b-modal id="modal-1" title="BootstrapVue" ref="cancel-modal" @hide="close">
      <template v-slot:modal-footer>
        <div class="w-100"></div>
      </template>
      <b-form @submit.stop.prevent="onSubmitCancel" v-if="show">
        <b-form-group class="mb-0" label="Nội dung hủy" label-for="textarea-note" description>
          <b-form-textarea
            name="note"
            id="textarea-note"
            placeholder="Nhập nội dung hủy"
            v-model="formCancel.note"
            v-validate="{required:true}"
            :state="validateState('note')"
          ></b-form-textarea>
          <b-form-invalid-feedback id="NoteFeedback">Nội dung không được để trống!</b-form-invalid-feedback>
        </b-form-group>
        <br />
        <b-form-group>
          <b-row>
            <b-col>
              <b-button block type="submit" variant="success">Xác nhận hủy</b-button>
            </b-col>
            <b-col>
              <b-button block variant="danger" @click.prevent="exitCancel">Thoát</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </b-form>
    </b-modal>

    <b-modal
      id="modal-2"
      data-backdrop="static"
      data-toggle="modal"
      data-keyboard="false"
      title="Xác nhận thanh toán"
      ref="confirm-modal"
      @hide="close"
    >
      <template v-slot:modal-footer>
        <div class="w-100"></div>
      </template>
      <div v-if="formConfirm.step == 1">
        <b-form @submit.stop.prevent="onSubmitCancel">
          <b-form-group
            class="mb-0"
            label="Nhập mã xác nhận"
            label-for="textarea-confirm_otp"
            description
          >
            <b-form-input
              name="confirm_otp"
              id="textarea-note"
              placeholder="Nhập mã xác nhận"
              v-model="formConfirm.otp"
              v-validate="{required:true}" 
            ></b-form-input>
             <!--:state="validateState('confirm_otp')"-->         
            <b-form-invalid-feedback id="confirm_otpFeedback">Nội dung không được để trống!</b-form-invalid-feedback>
          </b-form-group>
          <br />
          <b-form-group>
            <b-row>
              <b-col>
                <b-button
                  block
                  type="submit"
                  variant="success"
                  @click.prevent="confirmOtp"
                >Xác nhận thanh toán</b-button>
              </b-col>
              <b-col>
                <b-button block variant="danger" @click.prevent="exitCancel">Thoát</b-button>
              </b-col>
            </b-row>
          </b-form-group>
        </b-form>
      </div>
      <!--show thông tin sau khi xác nhận-->
      <div v-if="formConfirm.step == 2">
        <label>Bạn đã thanh toán {{formConfirm.message}} số tiền {{formConfirm.amount}} cho số tài khoản {{formConfirm.account_number}}</label>
        <label>Mã giao dịch {{formConfirm.transaction_id}}</label>
        <b-form-group>
          <b-row>
            <b-col>
              <b-button block variant="danger" @click.prevent="exitCancel">Thoát</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </div>
    </b-modal>
  </div>
</template>

<script>
import apiHelper from "../../helper/call_api";
import utilsHelper from "../../helper/helper";

export default {
  name: "UserAccounts",
  data() {
    return {
      fields: [
        { key: "AccountNumber", label: "Số tài khoản" },
        { key: "Money", label: "Tiền nợ" },
        { key: "Description", label: "Lời nhắn" },
        { key: "action" }
      ],
      receivedDeptReminders: [],
      sentDeptReminders: [],
      formCancel: {
        Id: "",
        note: ""
      },
      show: true,
      formConfirm: {
        result: false,
        message: "thất bại",
        transaction_id: "0",
        step: 1,
        account_number: "0",
        amount: "0",
        otp: "0"
      }
    };
  },
  mounted: function() {
    this.getListDeptreminders();
  },
  methods: {
    getListDeptreminders() {
      let me = this;
      apiHelper
        .call_api(`Deptreminders`, "get", "")
        .then(res => {
          let response = res.data;
          if (response) {
            if (response.ReceivedDeptReminders.length > 0) {
              let isActive = false;
              response.ReceivedDeptReminders.forEach(function(item) {
                if (!item.IsCanceled) {
                  me.receivedDeptReminders.push({
                    isActive: !isActive,
                    AccountNumber: item.RequestorAccountNumber,
                    Money: item.Money,
                    Description: item.Description,
                    Id: item.Id
                  });
                }
              });
            }

            if (response.SentDeptReminders.length > 0) {
              let isActive = false;
              response.SentDeptReminders.forEach(function(item) {
                if (!item.IsCanceled) {
                  me.sentDeptReminders.push({
                    isActive: !isActive,
                    AccountNumber: item.RecipientAccountNumber,
                    Money: item.Money,
                    Description: item.Description,
                    Id: item.Id
                  });
                }
              });
            }
          }
        })
        .catch(err => {
          console.error(err);
          utilsHelper.showErrorMsg(me, "Lỗi hệ thống!");
        });
    },
    onCancel(row) {
      let dataRow = row.item;
      if (dataRow) {
        this.formCancel.Id = dataRow.Id;
        this.$refs["cancel-modal"].show();
      }
    },
    onSubmitCancel(evt) {
      let me = this;
      evt.preventDefault();
      this.$validator.validateAll().then(result => {
        if (!result) {
          return;
        }

        if (me.formCancel.Id) {
          let deptReminder = {
            Notes: me.formCancel.note
          };
          apiHelper
            .call_api(`Deptreminders/${me.formCancel.Id}`, "post", deptReminder)
            .then(res => {
              let removedDept = me.ReceivedDeptReminders.filter(
                x => x.Id === me.formCancel.Id
              );
              if (removedDept.length > 0) {
                let index = me.ReceivedDeptReminders.indexOf(removedDept[0]);
                if (index > -1) {
                  me.ReceivedDeptReminders.splice(index, 1);
                }
              } else {
                removedDept = me.SentDeptReminders.filter(
                  x => x.Id === me.formCancel.Id
                );
                let index = me.SentDeptReminders.indexOf(removedDept[0]);
                if (index > -1) {
                  me.SentDeptReminders.splice(index, 1);
                }
              }

              this.$refs["cancel-modal"].hide();
              utilsHelper.showSuccessfullMsg(me, "Hủy nhắc nợ thành công!");
            })
            .catch(err => {
              console.error(err);
              this.$refs["cancel-modal"].hide();
              utilsHelper.showErrorMsg(me, "Lỗi hệ thống!");
            });
        }
      });
    },
    exitCancel() {
      this.$refs["cancel-modal"].hide();
    },
    close() {
      this.formCancel.Id = "";
      this.formCancel.note = "";

      // Trick to reset/clear native browser form validation state
      this.show = false;
      this.$nextTick(() => {
        this.show = true;
      });
    },
    onResolve(row) {
      let dataRow = row.item;
      apiHelper
        .call_api(`Deptreminders/Checkout/${dataRow.Id}`, "post", {})
        .then(res => {
          this.$refs["confirm-modal"].show();
          console.log(res);
          if (res.status == 200) {
            this.formConfirm.message = "thành công";
            this.formConfirm.transaction_id = res.data;
            this.formConfirm.step = 1;
            this.formConfirm.account_number = dataRow.AccountNumber;
            this.formConfirm.amount = dataRow.Money;
          } else {
            utilsHelper.showErrorMsg(this, "Lỗi hệ thống!");
          }
        })
        .catch(err => {
          console.log(err);
          utilsHelper.showErrorMsg(this, "Lỗi hệ thống!");
        });
    },
    confirmOtp() {
      // call api xác nhận otp
      apiHelper
        .call_api(
          `Deptreminders/Confirm/${this.formConfirm.transaction_id}` +
            `?otp=${this.formConfirm.otp}`,
          "post",
          {}
        )
        .then(res => {
          console.log(res);
          if (res.data == true) {
            this.formConfirm.message = "thành công";
            this.formConfirm.step = 2;
          } else {
            this.formConfirm.message = "thất bại";
            this.formConfirm.step = 2;
          }
        })
        .catch(err => {
          utilsHelper.showErrorMsg(this, err);
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
    }
  }
};
</script>