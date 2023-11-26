import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/receive/asn'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAllInfo(query) {
    return request({
        url: baseUrl,
        method: 'get',
        params: query
    })
}

/**
 * 确认到货
 * @param {行数据Id} id 
 */
export function confirmArrive(id) {
    let url = `${baseUrl}/confirmArrive/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 导出待到货信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/arriveExport`
    await downFile(url)
}