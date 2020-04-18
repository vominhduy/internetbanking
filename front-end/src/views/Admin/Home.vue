<template>
  <div>
   <b-form>
      <b-form-group label="Tên đăng nhập:">
        <b-form-input
          type="text"
          required
          placeholder="Tên đăng nhập"
        ></b-form-input>
      </b-form-group>
      <b-form-group label="Email:">
        <b-form-input
          type="text"
          required
          placeholder="Họ"
        ></b-form-input>
      </b-form-group>
      <b-form-group label="Email:">
        <b-form-input
          type="text"
          required
          placeholder="Họ"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="Số điện thoại:">
        <b-form-input
          type="text"
          required
          placeholder="Số điện thoại"
        ></b-form-input>
      </b-form-group>

      <b-button type="submit" variant="primary">Submit</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  name: "AdminHome",
  components: {}
};
</script>
