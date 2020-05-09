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
        <b-label>{{info_transfer.message}}</b-label>
        <br />
        <b-label>Số tài khoản: {{info_transfer.destination_account_number}}</b-label>
        <br />
        <b-label>Số tiền: {{info_transfer.amount}}</b-label>
        <br />
        <b-label>Icon chỗ này nha :))</b-label>
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