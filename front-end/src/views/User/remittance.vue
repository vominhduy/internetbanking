<template>
  <div class="wrapper">
    <div class="header">
      <h4 class="title">Chuyển khoản</h4>
    </div>
    <hr />
    <div class="body">
      <b-form @submit.stop.prevent="onSubmit">
        <div class="info_user">
          <b-label>Thông tin tài khoản</b-label>
          <div class="form-group">
            <b-label for="usr">Họ tên</b-label>
            <b-input type="text" class="form-control" disabled value="Nguyễn Văn Nguyễn" />
          </div>
          <div class="form-group">
            <b-label for="usr">Số tài khoản</b-label>
            <b-input type="text" class="form-control" disabled value="01772819" />
          </div>
          <div class="form-group">
            <b-label for="usr">Số dư</b-label>
            <b-input type="text" class="form-control" disabled value="1.000.000" />
          </div>
          <b-row>
            <b-col>
              <b-button class="mb-2 float-right">
                Đổi tài khoản
                <b-icon icon="chevron-right"></b-icon>
              </b-button>
            </b-col>
          </b-row>
        </div>
        <div class="info_transfer">
          <b-card no-body>
            <b-tabs pills card content-class="mt-3" fill>
              <b-tab title="Trong DDPBank" active>
                <div class="form-group">
                  <b-label for="usr">Số tài khoản</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Số tiền</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Nội dung</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Hình thức thanh toán phí</b-label>
                  <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                </div>
                <b-row>
                  <b-col>
                    <b-button class="mb-2 float-right" variant="success">
                      Chuyển khoản
                      <b-icon icon="chevron-right"></b-icon>
                    </b-button>
                  </b-col>
                </b-row>
              </b-tab>
              <b-tab title="Liên ngân hàng">
                <div class="form-group">
                  <b-label for="usr">Chọn ngân hàng</b-label>
                  <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                </div>
                <div class="form-group">
                  <b-label for="usr">Số tài khoản</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Số tiền</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Nội dung</b-label>
                  <b-input type="text" class="form-control" id="usr" />
                </div>
                <div class="form-group">
                  <b-label for="usr">Hình thức thanh toán phí</b-label>
                  <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
                </div>
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
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "remittance",
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
.wrapper {
  /*
  border-radius: 10px;
  border-style: solid; */
  padding: 10px;
}

.header {
  padding-bottom: 5px;
}

.info_user {
  background-color: snow;
  padding: 5px 20px 5px 20px;
  /*border: rgb(179, 124, 230)*/
  border-style: groove;
  border-radius: 5px;
  margin-bottom: 10px;
  /*-webkit-box-shadow: -5px 5px 5px 0px rgba(194, 60, 194, 0.3);
  -moz-box-shadow: -5px 5 5px 0px rgba(194, 60, 194, 0.3);
  box-shadow: -5px 5px 5px 0px rgba(194, 60, 194, 0.3);
  margin: 0px 0px 20px 10px;*/
}

/*
.form-group > b-label {
  top: 18px;
  left: 10px;
  position: relative;
  background-color: rgb(179, 124, 230);
  padding: 2px 5px 2px 5px;
  font-size: 18px;
  border-radius: 5px;
  margin-bottom: 0px;
  color: white;
}
.form-group > input {
  text-align: right;
}

.form-group > select {
  text-align-last: right;
}*/
</style>
