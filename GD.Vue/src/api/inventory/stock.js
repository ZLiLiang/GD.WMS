import request from '@/utils/request'
import { downFile } from '@/utils/request'

const baseUrl = '/inventory/stock'

/**
 * 分页查询所有库存列表数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getStock(query) {
    let url = `${baseUrl}/stock`
    return request({
        url: url,
        method: 'get',
        params: query
    })
}

/**
 * 分页查询所有库位列表数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getLocationStock(query) {
    let url = `${baseUrl}/locationstock`
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
 * 导出库存列表数据
 */
export async function exportStock() {
    let url = `${baseUrl}/stockExport`
    await downFile(url)
}

/**
 * 导出库位列表数据
 */
export async function exportLocationStock() {
    let url = `${baseUrl}/locationStockExport`
    await downFile(url)
}