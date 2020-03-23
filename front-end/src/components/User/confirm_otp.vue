<template>
  <b-card>
    <b-form-group>
      <b-label>Mã xác nhận (OTP) đã được gửi đến email abc***@gmail.com của quý khách.</b-label>
      <br />
      <b-label for="usr">Nhập mã xác nhận:</b-label>
      <b-input type="text" class="form-control" id="usr" />
      <b-label for="usr">{{info_user.name}}:</b-label>
    </b-form-group>
  </b-card>
</template>

<script>
export default {
  name: "confirm_otp",
  props: ['info_user']
};
</script>

<style scoped>
</style>