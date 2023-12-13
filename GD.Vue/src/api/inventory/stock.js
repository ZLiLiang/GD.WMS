import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/inventory/stock'

/**
 * 分页查询所有sku选择数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getSkuSelect(query) {
    let url = `${baseUrl}/skuselect`
    return request({
        url: url,
        method: 'get',
        params: query
    })
}

/**
 * 分页查询所有库存选择数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getStockSelect(query) {
    let url = `${baseUrl}/stockselect`
    return request({
        url: url,
        method: 'get',
        params: query
    })
}