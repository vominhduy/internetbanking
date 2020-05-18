<template>
  <div class="HistoryDeptIn">
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
                <b-form-input
                  v-model="filter"
                  type="search"
                  id="filterInput"
                  placeholder="Tìm kiếm"
                ></b-form-input>
                <b-input-group-append>
                  <b-button :disabled="!filter" @click="filter = ''">Xóa</b-button>
                </b-input-group-append>
              </b-input-group>
            </b-form-group>
          </b-col>
        </b-row>

        <!-- Main table element -->
        <b-table
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

          <template v-slot:cell(actions)="row">
            <b-button
              size="sm"
              @click="info(row.item, row.index, $event.target)"
              class="mr-1"
            ><b-icon icon="document-text"></b-icon></b-button>
          </template>
        </b-table>
        <b-row>
          <b-col sm="6" md="6" class="my-1">
            <div></div>
          </b-col>
          <b-col sm="6" md="6" class="my-1">
            <b-pagination
              v-model="currentPage3"
              :total-rows="totalRows3"
              :per-page="perPage3"
              class="mt-4"
              align="right"
            ></b-pagination>
          </b-col>
        </b-row>

        <b-modal :id="infoModal.id" :title="infoModal.title" ok-only @hide="resetInfoModal">
          <b-form-group
                    label-cols-sm="12"
                    label-cols-md="4"
                    label="Tên"
                    label-for="mnemonicName">
                    <b-form-input
                        id="mnemonicName"
                        name="mnemonicName"
                        v-model="infoModal.content.Name">
                    </b-form-input>
                </b-form-group>
          <b-form-group
                    label-cols-sm="12"
                    label-cols-md="4"
                    label="Địa chỉ"
                    label-for="mnemonicName">
                    <b-form-input
                        id="id"
                        name="mnemonicName"
                        v-model="infoModal.content.Address">
                    </b-form-input>
                </b-form-group>
           
      
        </b-modal>
  </div>
</template>

<script>
import helper from "../../helper/call_api.js";
import moment from 'moment';
// import moment from '';
import axios from "axios";

export default {
  name: "HistoryDeptIn",
  props: {
    histories3: {
      type: Array
    }
  },
  watch: {
    histories3: function() {
      this.items = this.histories3
    }
  },
  data() {
    return {
      passMessage: "",
      responeMessage: "",
      payInfo: {
        UserName: "",
        AccountNumber: "",
        Money: 0
      },
      valueType: "",
      type: 1,
      Types: [
        { value: 1, text: "Tên đăng nhập" },
        { value: 2, text: "Số tài khoản" }
      ],
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
          label: "tên đăng nhập ",
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
          label: "Phone",
          sortable: true,
          sortDirection: "desc"
        },
         {
          key: "Email",
          label: "Email",
          sortable: true,
          sortDirection: "desc"
        },
        { key: "actions", label: "Edit", class: "text-center"}
      ],
      totalRows3: 1,
      currentPage3: 1,
      perPage3: 6,
      pageOptions: [1, 2, 5, 10, 15, 50,70],
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
          return { text: f.label, value: f.key };
        });
    }
  },
  mounted() {
    // Set the initial number of items
    this.totalRows3 = this.items.length;

    let config = {
        headers: {
            admin_key: '09411a3942454ec9b36e3bcaf1d69f22',
        }
    }

    axios
      .get('Administrators/Employees', config)
      .then(response => {
        //   (this.info = response)
        this.items = response.data;
      })
  },
  methods: {
    canceled() {
      this.payInfo = {};
      this.valueType = "";
      this.type = 1;
    },
    info(item, index, button) {
      this.infoModal.title = "Sửa tài khoản";
      this.infoModal.content = item;
      this.$root.$emit("bv::show::modal", this.infoModal.id, button);
      console.log(item.Id)

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
    resetInfoModal() {
      let config = {
          headers: {
              admin_key: '09411a3942454ec9b36e3bcaf1d69f22',
          }
      };
      var cloneObj = Object.assign({}, this.infoModal.content);
      delete cloneObj.Id;
      delete cloneObj.Password;
      delete cloneObj.Role;
      delete cloneObj.UserName;
      axios
      .put('https://localhost:44396/api/Administrators/Employees/' + this.infoModal.content.Id, cloneObj, config)
      .then(response => {
        if(response && response.data) {
          alert('Sua thanh cong');
        } else {
          alert('Sua that bai');
        }
        this.infoModal.title = "";
        this.infoModal.content = "";
      }).catch(error => alert(error));
    },
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.totalRows3 = filteredItems.length;
      this.currentPage3 = 1;
    }
  }
};
</script>
