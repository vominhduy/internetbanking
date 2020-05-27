<template>
  <div class="ListEmployee">
    <b-row>
      <b-col sm="5" md="6" class="my-1">
        <b-form-group
          label="Hiển thị"
          label-cols-sm="6"
          label-cols-md="4"
          label-cols-lg="3"
          label-align-sm="right"
          label-size="sm"
          label-for="perPageSelect"
          class="mb-0"
        >
          <b-form-select v-model="perPage3" id="perPageSelect" size="sm" :options="pageOptions"></b-form-select>
        </b-form-group>
      </b-col>
      <b-col lg="6" class="my-1">
        <b-form-group
          label="Tìm kiếm"
          label-cols-sm="3"
          label-align-sm="right"
          label-size="sm"
          label-for="filterInput"
          class="mb-0"
        >
          <b-input-group size="sm">
            <b-form-input v-model="filter" type="search" id="filterInput" placeholder="Tìm kiếm"></b-form-input>
            <b-input-group-append>
              <b-button :disabled="!filter" @click="filter = ''">Xóa</b-button>
            </b-input-group-append>
          </b-input-group>
        </b-form-group>
      </b-col>
    </b-row>

    <!-- Main table element -->
    <b-table
      id="my-table"
      ref="table"
      show-empty
      small
      stacked="md"
      :items="items"
      :fields="fields"
      :current-page="currentPage3"
      :per-page="perPage3"
      :filter="filter"
      :filterIncludedFields="filterOn"
      :sort-by.sync="sortBy"
      :sort-desc.sync="sortDesc"
      :sort-direction="sortDirection"
      @filtered="onFiltered"
    >
      <template v-slot:cell(name)="row">{{ row.value.first }} {{ row.value.last }}</template>
      <template v-slot:cell(name)="row">{{ row.value}}</template>
      <template v-slot:cell(actionsion)="row">
        <b-button size="sm" @click="hamxoa(row.item, row.index, $event.target)" variant="danger" class="mr-1">
          <b-icon icon="trash-fill" aria-hidden="true"></b-icon>
        </b-button>
      </template>

      <template v-slot:cell(actions)="row">
        <b-button size="sm" @click="info(row.item, row.index, $event.target)" class="mr-1">
          <b-icon icon="gear-fill" aria-hidden="true"></b-icon>
        </b-button>
      </template>
    </b-table>
    <b-row>
      <b-col sm="6" md="6" class="my-1">
        <div></div>
      </b-col>
      <b-col sm="6" md="6" class="my-1">
        <b-pagination
          aria-controls="my-table"
          v-model="currentPage3"
          :total-rows="totalRows3"
          :per-page="perPage3"
          class="mt-4"
          align="right"
        ></b-pagination>
      </b-col>
    </b-row>

    <b-modal :id="infoModal.id" :title="infoModal.title" ok-only @hide="resetInfoModal">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên" label-for="mnemonicName">
        <b-form-input v-model="infoModal.content.Name"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Địa chỉ" label-for="mnemonicName">
        <b-form-input v-model="infoModal.content.Address"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Phone" label-for="mnemonicName">
        <b-form-input v-model="infoModal.content.Phone"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Email" label-for="mnemonicName">
        <b-form-input v-model="infoModal.content.Email"></b-form-input>
      </b-form-group>
    </b-modal>
  </div>
</template>

<script>
import helper from "../../helper/call_api.js";
import moment from "moment";
import apiHelper from "../../helper/call_api";
// import moment from '';
import axios from "axios";

export default {
  name: "ListEmployee",
  data() {
    return {
      items: [],
      fields: [
        {
          key: "Name",
          label: "Tên",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "Username",
          label: "Tên đăng nhập ",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "Address",
          label: "Địa chỉ",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "Phone",
          label: "Số điện thoại",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "Email",
          label: "Email",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "actions",
          label: "Sửa",
          class: "text-center"
        },
        {
          key: "actionsion",
          label: "Xóa",
          class: "text-center"
        }
      ],
      totalRows3: 1,
      currentPage3: 1,
      perPage3: 5,
      pageOptions: [1, 2, 5, 10, 15, 50, 70],
      sortBy: "",
      sortDesc: false,
      sortDirection: "asc",
      filter: null,
      filterOn: [],
      infoModal: {
        id: "info-modal-Dept-In",
        title: "",
        content: ""
      }
    };
  },
  computed: {
    sortOptions() {
      // Create an options list from our fields
      return this.fields
        .filter(f => f.sortable)
        .map(f => {
          return {
            text: f.label,
            value: f.key
          };
        });
    }
  },
  mounted() {
    // Set the initial number of items
    this.totalRows3 = this.items.length;

    apiHelper.call_api("Administrators/Employees", "get").then(response => {
      //   (this.info = response)
      this.items = response.data;
      this.totalRows3 = this.items.length;
    });
  },
  methods: {
    info(item, index, button) {
      this.infoModal.title = "Chỉnh sửa tài khoản";
      this.infoModal.content = item;
      this.$root.$emit("bv::show::modal", this.infoModal.id, button);
      console.log(item.Id);

      //   let config = {
      //     headers: {
      //         admin_key: '09411a3942454ec9b36e3bcaf1d69f22',
      //     }
      // },
      // axios.post('https://localhost:44396/api/Administrators/Employees', {
      //   firstName: 'First name',
      //   lastName: 'Last name'
      // })
      // .then(function (response) {
      //   console.log(response);
      // })
      // .catch(function (error) {
      //   console.log(error);
      // });
      //   axios
      //   .put('https://localhost:44396/api/Administrators/Employees', config)
      //   .then(response => {
      //     //   (this.info = response)
      //     this.items = response.data;
      //   })
    },
    hamxoa(item, index, button) {
      // this.infoModal.title = "Sửa tài khoản";
      this.infoModal.content = item;
      //this.$root.$emit("bv::show::modal", this.infoModal.id, button);

      //    let config = {
      //       headers: {
      //           admin_key: '09411a3942454ec9b36e3bcaf1d69f22',
      //       }
      //   };
      //  console.log('id la'+ this.infoModal.content.Id)

      //   axios
      //   .delete('Administrators/Employees/' + this.infoModal.content.Id,config)
      //   .then(response => {
      //       axios
      //      .get('Administrators/Employees', config)
      //   .then(response => {
      //     //   (this.info = response)
      //     this.items = response.data;
      //   })
      //      console.log(response)
      //   }).catch(error => alert(error));

      apiHelper
        .call_api(
          "Administrators/Employees/" + this.infoModal.content.Id,
          "delete"
        )
        .then(response => {
          if (response && response.data) {
              this.items.splice(this.items.find(x => x.Id == item.Id)[0], 1);
              this.totalRows3 = this.items.length;
                //this.currentPage3 = 1;
              this.$refs.table.refresh()
            apiHelper
              .call_api("Administrators/Employees", "get")
              .then(response => {
                //   (this.info = response)
                this.items = response.data;
              });

            this.makeToast("success", "Xóa thành công!");
          } else {
            this.makeToast("danger", "Xóa thất bại!");
          }

          this.infoModal.title = "";
          this.infoModal.content = "";
        })
        .catch(error => this.makeToast("danger", error));
    },
    resetInfoModal() {
      let config = {
        headers: {
          admin_key: "09411a3942454ec9b36e3bcaf1d69f22"
        }
      };
      var cloneObj = Object.assign({}, this.infoModal.content);
      delete cloneObj.Id;
      delete cloneObj.Password;
      delete cloneObj.Role;
      delete cloneObj.UserName;
      // axios
      //     .put('Administrators/Employees/' + this.infoModal.content.Id, cloneObj, config)
      //     .then(response => {
      //         if (response && response.data) {
      //             alert('Sua thanh cong');
      //         } else {
      //             alert('Sua that bai');
      //         }
      //         this.infoModal.title = "";
      //         this.infoModal.content = "";
      //     }).catch(error => alert(error));
      apiHelper
        .call_api(
          "Administrators/Employees/" + this.infoModal.content.Id,
          "put",
          cloneObj
        )
        .then(response => {
          if (response && response.data) {
            this.makeToast("success", "Sửa thông tin thành công!");
            //alert('Sua thanh cong');
          } else {
            this.makeToast("danger", "Sửa thông tin thất bại!");
            //alert('Sua that bai');
          }
          this.infoModal.title = "";
          this.infoModal.content = "";
        })
        .catch(error => this.makeToast("danger", error));
    },
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.totalRows3 = filteredItems.length;
      this.currentPage3 = 1;
    },
    makeToast(variant = null, content = null) {
      this.$bvToast.toast(content, {
        title: "Thông báo!",
        autoHideDelay: 3000,
        variant: variant,
        solid: true,
        toaster: "b-toaster-bottom-right"
      });
    }
  }
};
</script>
<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
.delete-color {
  background-color: #dc3545;
}
</style>
