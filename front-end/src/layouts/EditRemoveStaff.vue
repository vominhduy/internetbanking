<template>
  <div>
    <H1>Admin Layout</H1>
    <NavBar />
    <b-container class="mt-4">
      <b-row>
        <b-col sm="3">
          <SideBar />
        </b-col>
        <b-col sm="9">
          <slot />
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import NavBar from '@/components/NavBar.vue';
import SideBar from '@/components/Admin/SideBar.vue';
    export default {
        name: "EditRemoveStaffLayout",
        components:{
            NavBar, SideBar
        }
    }
</script>