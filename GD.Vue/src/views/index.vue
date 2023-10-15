<template>
  <div class="home">
    <!-- 用户信息 -->
    <el-row :gutter="15">
      <el-col :md="24" :lg="16" :xl="24" class="mb10">
        <el-card shadow="hover">
          <div class="user-item">
            <div class="user-item-left">
              <el-avatar :size="60" shape="circle" :src="userInfo.avatar" />
            </div>

            <div class="user-item-right">
              <el-row>
                <el-col :xs="24" :md="24" class="right-title mb20 one-text-overflow">
                  <div class="mb10">
                    {{ userInfo.welcomeMessage }} <strong>{{ userInfo.nickName }}</strong>
                    <span>({{ userInfo.welcomeContent }})</span>
                  </div>
                </el-col>
              </el-row>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%">
          <div class="text-warning mb10">{{ currentTime }} {{ weekName }}</div>
          <div class="work-wrap">
            <div>HELLOW</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="32">
      <el-col :xs="24" :sm="24" :lg="24">
        <div class="chart-wrapper">
          <lineChart :chart-data="lineChartData" :key="dataType" />
        </div>
      </el-col>
    </el-row>

    <el-row :gutter="32">
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <raddarChart />
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <pieChart />
        </div>
      </el-col>
      <el-col :xs="24" :sm="24" :lg="8">
        <div class="chart-wrapper">
          <barChart />
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup name="index">
import lineChart from './dashboard/LineChart.vue'
import raddarChart from './dashboard/RaddarChart.vue'
import pieChart from './dashboard/PieChart.vue'
import barChart from './dashboard/BarChart.vue'
// import WordCloudChat from './dashboard/WordCloud.vue'
import dayjs from 'dayjs'
// 时间插件
import duration from 'dayjs/plugin/duration'
dayjs.extend(duration)

import useUserStore from '@/store/modules/user'
import { getWeek, parseTime } from '@/utils/ruoyi'

const data = {
  newVisitis: {
    expectedData: [100, 120, 161, 134, 105, 160, 165],
    actualData: [120, 82, 91, 154, 162, 140, 145]
  },
  messages: {
    expectedData: [200, 192, 120, 144, 160, 130, 140],
    actualData: [180, 160, 151, 106, 145, 150, 130]
  },
  purchases: {
    expectedData: [80, 100, 121, 104, 105, 90, 100],
    actualData: [120, 90, 100, 138, 142, 130, 130]
  },
  shoppings: {
    expectedData: [130, 140, 141, 142, 145, 150, 160],
    actualData: [120, 82, 91, 154, 162, 140, 130]
  }
}

const userInfo = computed(() => {
  return useUserStore().userInfo
})
const currentTime = computed(() => {
  return parseTime(new Date(), 'YYYY-MM-DD')
})
const weekName = getWeek()

let lineChartData = reactive([])
const dataType = ref(null)
function handleSetLineChartData(type) {
  dataType.value = type
  lineChartData = data[type]
}
handleSetLineChartData('newVisitis')

</script>

<style lang="scss" scoped>
.home {
  .home-card-more {
    float: right;
    padding: 3px 0;
    font-size: 13px;
  }

  .user-item {
    // height: 198px;
    display: flex;
    align-items: center;

    .user-item-left {
      width: 60px;
      height: 60px;
      overflow: hidden;
      margin-right: 10px;
    }

    .user-item-right {
      flex: 1;

      .right-title {
        font-size: 20px;
      }
    }
  }

  .info {
    height: 200px;
    // overflow-y: scroll;
  }

  .work-wrap {
    display: grid;
    grid-template-columns: repeat(2, 50%);

    .item {
      text-align: center;

      .name {
        color: #606666;
      }
    }
  }
}

.chart-wrapper {
  background: var(--base-bg-main);
  padding: 16px 16px 0;
  margin-bottom: 32px;
}

@media (max-width: 1024px) {
  .chart-wrapper {
    padding: 8px;
  }
}
</style>
