<template>
  <b-card class="bcard-shadow">
    <div class="header">
      <h4 class="title">
        <b-icon icon="chevron-right"></b-icon>&nbsp;Thông tin
      </h4>
      <hr />
    </div>
    <div class="body">
      <b-form-group>
        <!-- <div v-if="info_transfer.is_success == true" style="font-size:200px; ">
          <b-icon icon="check-circle"></b-icon>
        </div>
        <div v-else style="font-size:200px;">
          <b-icon icon="x-circle"></b-icon>
        </div> -->

        <label>{{info_transfer.message}}</label>
        <br />
        <label>Số tài khoản: {{info_transfer.destination_account_number}}</label>
        <br />
        <label>Số tiền: {{info_transfer.amount}}</label>
        <br />
      </b-form-group>
    </div>
  </b-card>
</template>

<script>
export default {
  name: "transfer_money_info",
  props: ["info_transfer"]
};
</script>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>