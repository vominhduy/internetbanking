<template>
  <div>
    <b-card-group v-if="show==false">
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
            <b-form-group>
              <label for="usr">Họ tên</label>
              <b-input type="text" class="form-control" disabled v-model="user.Name" />
            </b-form-group>
            <b-form-group>
              <label for="usr">Số tài khoản</label>
              <b-input type="text" class="form-control" disabled value="01772819" />
            </b-form-group>
            <b-form-group>
              <label for="usr">Số dư</label>
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
          <label>Thông tin người nhận</label>
          <b-form @submit.stop.prevent="onSubmit">
            <div class="info_transfer">
              <b-card no-body>
                <b-tabs pills card content-class="mt-3" fill>
                  <b-tab title="Trong DDPBank" active>
                    <b-form-group>
                      <label for="usr">Số tài khoản</label>
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
                      <label for="usr">Số tiền</label>
                      <b-input type="text" class="form-control" id="usr" />
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Nội dung</label>
                      <b-input type="text" class="form-control" id="usr" />
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Hình thức thanh toán phí</label>
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
                      <label for="usr">Chọn ngân hàng</label>
                      <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Số tài khoản</label>
                      <b-input type="text" class="form-control" id="usr" />
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Số tiền</label>
                      <b-input type="text" class="form-control" id="usr" />
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Nội dung</label>
                      <b-input type="text" class="form-control" id="usr" />
                    </b-form-group>
                    <b-form-group>
                      <label for="usr">Hình thức thanh toán phí</label>
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
    </b-card-group>

    <component v-if="show==true" v-bind:is="component" v-bind:info_user="info_user" />
    <button @click.prevent="changeComponet">Đổi componet</button>
  </div>
</template>

<script>
import NavBar from "@/components/User/confirm_otp.vue";
import Categories from "@/components/User/transfer_money_info.vue";
import axios from "axios";
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
      info_user: {
        name: "phong nha",
        account_number: 1234
      },
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
  mounted(){
      var usertest ={
         Phone: "2",
        Name: "3",
        Email: "4",
        Address: "5",
        Gender: 1
      }
      this.user = usertest
  },
  methods: {
    changeComponet() {
      this.show = true;
      console.log(this.component);
      this.component = "NavBar";
    },
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