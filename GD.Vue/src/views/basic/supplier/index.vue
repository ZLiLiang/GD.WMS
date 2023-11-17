<template>
  <div class="app-container">
    <!-- 搜索功能 -->
    <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
      <el-form-item label="供应商名称" prop="supplierName">
        <el-input v-model="queryParams.supplierName" placeholder="请输入供应商名称" clearable style="width: 160px" />
      </el-form-item>
      <el-form-item label="负责人" prop="manager">
        <el-input v-model="queryParams.manager" placeholder="请输入负责人" clearable style="width: 160px" />
      </el-form-item>
      <el-form-item label="联系方式" prop="contactTel">
        <el-input v-model="queryParams.contactTel" placeholder="请输入联系方式" clearable style="width: 160px" />
      </el-form-item>
      <el-form-item label="创建时间">
        <el-date-picker v-model="dateRange" style="width: 300px" type="daterange" range-separator="-"
          start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
        <el-button icon="refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <!-- 工具栏 -->
    <el-row :gutter="10">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="upload" @click="handleImport">导入</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
    </el-row>

    <!-- 表格 -->
    <el-table v-loading="loading" :data="supplierList" highlight-current-row>
      <el-table-column label="供应商名称" prop="supplierName" align="center" v-if="columns.showColumn('supplierName')" />
      <el-table-column label="所在城市" prop="city" align="center" v-if="columns.showColumn('city')" />
      <el-table-column label="详细地址" prop="address" align="center" v-if="columns.showColumn('address')" />
      <el-table-column label="负责人" prop="manager" align="center" v-if="columns.showColumn('manager')" />
      <el-table-column label="Email" prop="email" align="center" v-if="columns.showColumn('email')" />
      <el-table-column label="联系方式" prop="contactTel" align="center" v-if="columns.showColumn('contactTel')" />
      <el-table-column label="创建人" prop="createBy" align="center" v-if="columns.showColumn('createBy')" />
      <el-table-column label="创建时间" prop="createTime" align="center" width="170"
        v-if="columns.showColumn('createTime')" />
      <el-table-column label="最后更新时间" prop="updateTime" align="center" width="170"
        v-if="columns.showColumn('updateTime')" />

      <!-- 操作行 -->
      <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
        <template #default="scope">
          <div>
            <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
            </el-button>
            <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
            </el-button>
          </div>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页栏 -->
    <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize"
      @pagination="getList" />

    <!-- 对话框 -->
    <el-dialog :title="title" v-model="open" width="500px" :show-close="false" :draggable="true">
      <el-form :model="form" :rules="rules" ref="supplierRef" label-width="100px">
        <el-form-item label="供应商名称" prop="supplierName">
          <el-input v-model="form.supplierName" placeholder="请输入供应商名称" />
        </el-form-item>
        <el-form-item label="所在城市" prop="city">
          <el-input v-model="form.city" placeholder="请输入所在城市" />
        </el-form-item>
        <el-form-item label="详细地址" prop="address">
          <el-input v-model="form.address" placeholder="请输入详细地址" />
        </el-form-item>
        <el-form-item label="负责人" prop="manager">
          <el-input v-model="form.manager" placeholder="请输入负责人" />
        </el-form-item>
        <el-form-item label="Email" prop="email">
          <el-input v-model="form.email" placeholder="请输入Email" />
        </el-form-item>
        <el-form-item label="联系方式" prop="contactTel">
          <el-input v-model="form.contactTel" placeholder="请输入联系方式" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button text @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">提交</el-button>
      </template>
    </el-dialog>

    <!-- 导入操作对话框 -->
    <el-dialog :title="upload.title" v-model="upload.open" width="500px" :show-close="false" :draggable="true">
      <el-upload name="file" ref="uploadRef" :limit="1" accept=".xlsx,.xls" :headers="upload.headers" :action="upload.url"
        :on-progress="handleFileUploadProgress" :on-success="handleFileSuccess" :auto-upload="false" drag>
        <el-icon class="el-icon--upload">
          <upload-filled />
        </el-icon>
        <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
        <template #tip>
          <div class="el-upload__tip text-center">
            <span>仅允许导入xls、xlsx格式文件。</span>
            <el-link type="primary" :underline="false" style="font-size: 12px; vertical-align: baseline"
              @click="importTemplate">下载模板</el-link>
          </div>
        </template>
      </el-upload>

      <template #footer>
        <el-button @click="upload.open = false">取消</el-button>
        <el-button type="primary" @click="submitFileForm">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { getToken } from "@/utils/auth";
import { getAll, getSupplierInfo, addSupplierInfo, editSupplierInfo, deleteById, exportSupplier, exportTemplate } from "@/api/warehousManagement/supplier";

// 总条数
const total = ref(0);
// 展示搜索界面
const showSearch = ref(false);
// 加载...
const loading = ref(true);
// 供应商列表
const supplierList = ref([]);
// 展示对话框
const open = ref(false);
// 对话框标题
const title = ref("");
// 时间范围
const dateRange = ref([]);
// 用户导入参数
const upload = reactive({
  // 是否显示弹出层（用户导入）
  open: false,
  // 弹出层标题（用户导入）
  title: "",
  // 是否禁用上传
  isUploading: false,
  // 设置上传的请求头部
  headers: { Authorization: "Bearer " + getToken() },
  // 上传的地址
  url: import.meta.env.VITE_APP_BASE_API + "/warehousemanagement/supplier/importData",
});
// 数据
const data = reactive({
  form: {
    supplierId: undefined,
    supplierName: undefined,
    city: undefined,
    address: undefined,
    manager: undefined,
    email: undefined,
    contactTel: undefined,
  },
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    supplierName: undefined,
    city: undefined,
    address: undefined,
    manager: undefined,
    email: undefined,
    contactTel: undefined,
  },
  rules: {
    supplierName: [
      { required: true, message: "供应商名称不能为空", trigger: "blur" },
    ],
    manager: [{ required: true, message: "负责人不能为空", trigger: "blur" }],
    contactTel: [
      {
        required: true,
        message: "电话格式:固话或13、14、15、17、18开头+9位阿拉伯数字",
        trigger: "blur",
        pattern: /^((0\d{2,3}-\d{7,8})|(1[34578]\d{9}))$/,
      },
    ],
  },
});
// 列显隐信息
const columns = ref([
  { key: 0, label: `供应商名称`, visible: true, prop: "supplierName" },
  { key: 1, label: `所在城市`, visible: true, prop: "city" },
  { key: 2, label: `详细地址`, visible: true, prop: "address" },
  { key: 3, label: `负责人`, visible: true, prop: "manager" },
  { key: 4, label: `Email`, visible: true, prop: "email" },
  { key: 5, label: `联系方式`, visible: true, prop: "contactTel" },
  { key: 6, label: `创建人`, visible: false, prop: "createBy" },
  { key: 7, label: `创建时间`, visible: true, prop: "createTime" },
  { key: 8, label: `最后更新时间`, visible: false, prop: "updateTime" },
  { key: 9, label: `操作`, visible: true, prop: "operate" },
]);
// 表单、搜索参数、规则
const { form, queryParams, rules } = toRefs(data);
// 当时实例
const { proxy } = getCurrentInstance();

/**
 * 重置操作表单
 */
function reset() {
  form.value = {
    supplierId: undefined,
    supplierName: undefined,
    city: undefined,
    address: undefined,
    manager: undefined,
    email: undefined,
    contactTel: undefined,
  };
  proxy.resetForm('supplierRef')
}

/**
 * 获取列表
 */
function getList() {
  loading.value = true;
  let params = proxy.addDateRange(queryParams.value, dateRange.value);
  getAll(params).then(res => {
    loading.value = false
    supplierList.value = res.data.result
    total.value = res.data.totalNum
  })
}

/**
 * 搜索按钮操作
 */
function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

/**
 * 重置按钮操作
 */
function resetQuery() {
  dateRange.value = [];
  proxy.resetForm("queryRef");
  handleQuery();
}

/**
 * 新增按钮操作
 */
function handleAdd() {
  reset()
  open.value = true;
  title.value = "新增供应商信息";
}

/**
 * 导入按钮操作
 */
function handleImport() {
  upload.title = "供应商导入";
  upload.open = true;
}

/**
 * 导出按钮操作
 */
function handleExport() {
  proxy
    .$modal
    .confirm('是否确认导出所有供应商数据项?', '警告', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    .then(async () => {
      await exportSupplier()
    })
}

/**
 * 提交按钮
 */
function submitForm() {
  proxy.$refs["supplierRef"].validate((valid) => {
    if (valid) {
      if (form.value.supplierId != undefined) {
        editSupplierInfo(form.value.supplierId, form.value).then(res => {
          proxy.$modal.msgSuccess('修改成功')
          open.value = false
          getList()
        })
      } else {
        addSupplierInfo(form.value).then(res => {
          proxy.$modal.msgSuccess('新增成功')
          open.value = false
          getList()
        })
      }
    }
  });
}

/**
 * 取消按钮
 */
function cancel() {
  reset();
  open.value = false;
}

/**
 * 修改按钮操作
 * @param {行数据} row
 */
function handleUpdate(row) {
  reset()
  const supplierId = row.supplierId
  getSupplierInfo(supplierId).then(response => {
    form.value = response.data
    open.value = true
    title.value = '修改供应商'
  })
  open.value = true
}

/**
 * 删除按钮操作
 * @param {行数据} row
 */
function handleDelete(row) {
  const supplierId = row.supplierId
  proxy
    .$modal
    .confirm('是否确认删除用户编号为"' + row.supplierName + '"的数据项？')
    .then(() => {
      return deleteById(supplierId)
    })
    .then(() => {
      getList()
      proxy.$modal.msgSuccess('删除成功')
    })
    .catch(() => { })
}

/**
 * 文件上传中处理
 * @param {事件} event
 * @param {文件} file
 * @param {文件列表} fileList
 */
function handleFileUploadProgress(event, file, fileList) {
  upload.isUploading = true;
}

/**
 * 文件上传成功处理
 * @param {响应内容} response
 * @param {文件} file
 * @param {文件列表} fileList
 */
function handleFileSuccess(response, file, fileList) {
  const { code, msg, data } = response;
  upload.open = false;
  upload.isUploading = false;
  proxy.$refs["uploadRef"].clearFiles();
  proxy
    .$modal
    .confirm('导入结果:' + data.item1)
    .then(() => {
      getList();
    })
}

/**
 * 下载模板操作
 */
async function importTemplate() {
  await exportTemplate()
}

/**
 * 提交上传文件
 */
function submitFileForm() {
  proxy.$refs["uploadRef"].submit();
}

getList();
</script>

<style></style>