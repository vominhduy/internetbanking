<template>
  <b-card-group>
    <b-card>
      <div class="header">
        <h4 class="title">
          <b-icon icon="chevron-right"></b-icon>&nbsp;Chuyển khoản
        </h4>
        <hr />
      </div>
      <div class="body">
        <div class="info_user">
          <b-label>Thông tin tài khoản</b-label>
          <b-form-group>
            <b-label for="usr">Họ tên</b-label>
            <b-input type="text" class="form-control" disabled value="Nguyễn Văn Nguyễn" />
          </b-form-group>
          <b-form-group>
            <b-label for="usr">Số tài khoản</b-label>
            <b-input type="text" class="form-control" disabled value="01772819" />
          </b-form-group>
          <b-form-group>
            <b-label for="usr">Số dư</b-label>
            <b-input type="text" class="form-control" disabled value="1.000.000" />
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
        <b-label>Thông tin người nhận</b-label>
        <b-form @submit.stop.prevent="onSubmit">
          <div class="info_transfer">
            <b-card no-body>
              <b-tabs pills card content-class="mt-3" fill>
                <b-tab title="Trong DDPBank" active>
                  <b-form-group>
                    <b-label for="usr">Số tài khoản</b-label>
                    <b-input-group>
                      <b-form-input type="text" class="form-control" id="usr" />
                      <b-input-group-append>
                        <b-button variant="success" size="sm">
                          Chọn người nhận
                          <b-icon icon="document-text"></b-icon>
                        </b-button>
                      </b-input-group-append>
                    </b-input-group>
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Số tiền</b-label>
                    <b-input type="text" class="form-control" id="usr" />
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Nội dung</b-label>
                    <b-input type="text" class="form-control" id="usr" />
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Hình thức thanh toán phí</b-label>
                    <b-form-select
                      class="select"
                      id="Gender"
                      v-model="user.Gender"
                      :options="Genders"
                    ></b-form-select>
                  </b-form-group>
                  <b-row>
                    <b-col>
                      <b-button v-b-modal.modal-center class="mb-2 float-right" variant="success">
                        Chuyển khoản
                        <b-icon icon="chevron-right"></b-icon>
                      </b-button>
                    </b-col>
                  </b-row>
                </b-tab>
                <b-tab title="Liên ngân hàng">
                  <b-form-group>
                    <b-label for="usr">Chọn ngân hàng</b-label>
                    <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Số tài khoản</b-label>
                    <b-input type="text" class="form-control" id="usr" />
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Số tiền</b-label>
                    <b-input type="text" class="form-control" id="usr" />
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Nội dung</b-label>
                    <b-input type="text" class="form-control" id="usr" />
                  </b-form-group>
                  <b-form-group>
                    <b-label for="usr">Hình thức thanh toán phí</b-label>
                    <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                  </b-form-group>
                  <b-row>
                    <b-col>
                      <b-button class="mb-2 float-right" variant="success">
                        Chuyển khoản
                        <b-icon icon="chevron-right"></b-icon>
                      </b-button>
                    </b-col>
                  </b-row>
                </b-tab>
              </b-tabs>
            </b-card>
          </div>
        </b-form>
      </div>
    </b-card>
    <b-modal id="modal-center" no-close-on-backdrop="true" centered title="Xác nhận OTP">
      <b-form-group>
        <b-label>Mã xác nhận (OTP) đã được gửi đến email abc***@gmail.com của quý khách.
        </b-label>
        <br>
        <b-label for="usr">Nhập mã xác nhận: </b-label>
        <b-input type="text" class="form-control" id="usr" />
      </b-form-group>
    </b-modal>
  </b-card-group>
</template>

<script>
import axios from "axios";
export default {
  name: "transfer_money",
  data() {
    return {
      respone: {},
      user: {
        Phone: "",
        Name: "",
        Email: "",
        Address: "",
        Gender: 1
      },
      Genders: [
        { value: 1, text: "Nam" },
        { value: 2, text: "Nữ" },
        { value: 3, text: "Khác" }
      ]
    };
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      axios
        .post(`employees`, this.user)
        .then(res => {
          this.respone = res.data;
          this.$refs["respone"].show();
        })
        .catch(err => {
          this.empty = true;
          console.log(err);
        });
    },
    canceled() {
      this.user = {};
      this.user.Gender = 1;
    }
  }
};
</script>
<style scoped>
</style>
