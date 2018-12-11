# -*- coding: utf-8 -*-


import sys
import json
import md5
import subprocess
import os
import consts
import urlparse
import hashlib
import collections
reload(sys)
sys.setdefaultencoding('utf8')
import bottle
from bottle import request, response, run
from game_server import game_server_manager
import requests
from urllib import unquote, quote

from db import default_db as db
import setting
from logger import log


server = application = bottle.default_app()


def _get_order(order_id):
    sql = 'select * from `order` where id=%s'
    order = db.select_one(sql, [order_id])
    log.info('get order(%s) %s', order_id, order)
    return order


def _update_order_error(order_id, amount):
    sql = """update `order` set `status`= 2,amount = %s where id = %s"""
    db.execute(sql, [amount, order_id])


def check_price(order, amount):
    return True
    if int(order['price']) / 100 <= amount:
        return True
    else:
        log.warn("price not equal %s != %s", order['price'] / 100, amount)
        _update_order_error(order['id'],amount)
        return False


def _update_order_status(server_id, order_id, platform_order_id, content, ip, amount=0):
    '''
    更新游戏服务器订单状态
    '''
    # 更新订单状态为成功
    sql = """update `order`
                set status=%s,platform_order_id=%s,
                callback_data=%s,
                callback_time=now(),
                callback_ip=%s,
                amount = %s
                where id=%s
                and status != %s
                and status != %s
                """
    result = db.execute(sql, [consts.ORDER_STATUS_SUCCESS,
                              platform_order_id, content,
                              ip, amount, order_id,
                              consts.ORDER_STATUS_SUCCESS,
                              consts.ORDER_STATUS_FINISH])
    log.info('update `order` to %s, result %s',
             consts.ORDER_STATUS_SUCCESS, result)
    gs = game_server_manager.game_servers[server_id]
    if gs:
        # 通知到游戏服务器 订单完成
        result, data = gs.call_game_server('SendOrderGoods', order_id)
        if result:
            sql = """update .`order`
                set status=%s
                where id=%s and status=%s
                """
            db.execute(sql, [consts.ORDER_STATUS_FINISH,
                             order_id, consts.ORDER_STATUS_SUCCESS])

        log.info('call_gameserver pay result %s', result)
    else:
        log.info('call_gameserver pay game_server(%s) not found', server_id)

    return result




@bottle.route('/pay/iran_callback', method='POST')
def iran_callback():
    # funpax_keys = {
	# "10725414":'bb01d5678ae34db1ae9ad20c84ab6a9f',
	# "10133353":'eab9903fd42a92168363cbd73102ba9e',
    #     "10356928":"4183c599f0ee434ef1086f036a12e562",
    # }
    # body = request.body.getvalue()
    # log.info('post data:%s', body)

    # sign = request.params.get('sign')
    # funpax_order_id = request.params.get('orderId')
    # app_id = request.params.get('appId')
    # app_key = funpax_keys.get(app_id,"")
    # product_id = request.params.get('productId')
    # amount = request.params.get('productNum')
    # price = request.params.get('dealPrice')
    # order_id = request.params.get('extInfo')
    # funpax_user_id = request.params.get('userId')

    # s = 'appId=%s&dealPrice=%s&extInfo=%s&orderId=%s&productId=%s&productNum=%s&userId=%s%s' % \
    #         (app_id, price, order_id, funpax_order_id, product_id, amount, funpax_user_id, app_key)

    # our_sign = md5.new(s).hexdigest()
    # if our_sign != sign:
    #     log.warning('funpax sign invalid.')
    #     return 'sign failed.'


	sdk_order_id = request.params.get('sdkOrderId')
	order_id = request.params.get('orderId')
	price = request.params.get('price')
	
    order_id = int(order_id)
    order = _get_order(order_id)

    if not order:
        log.warning('order not found.[id=%s]' % order_id)
    else:
        if order['status'] < consts.ORDER_STATUS_FINISH:
            server_id = order['server_id']
            if check_price(order, int(price)):
                content = json.dumps(
                    {k: v for (k, v) in request.params.items()})
                _update_order_status(
                    server_id, order_id, sdk_order_id, content, request.remote_addr)
                log.info('pay success.[order_id=%s]' % order_id)
                return 'success'
            else:
                log.warning('money is invalid.[id=%s]' % order_id)
        else:
            log.warning('order status is invalid.[id=%s]' % order_id)

    return 'failure'


def gen_md5(code):
    m = hashlib.md5()
    m.update(code)
    return m.hexdigest()



if __name__ == '__main__':
    from gevent.wsgi import WSGIServer
    WSGIServer((setting.HOST, setting.PORT), application).serve_forever()
    #run(host=setting.HOST, port=setting.PORT, app=application)

