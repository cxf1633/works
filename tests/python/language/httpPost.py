#!/usr/bin/env python
#-*- coding: utf-8 -*-

import urllib
import urllib2



def httpPost(requrl, data):
    # requrl = "http://testpay.baziline.ws/pay/iran_callback"
    # test_data = {'orderId':'259','extInfo':'3002', 'productNum':'1'}
    test_data_urlencode = urllib.urlencode(data)

    req = urllib2.Request(url = requrl,data =test_data_urlencode)
    print req

    res_data = urllib2.urlopen(req)
    res = res_data.read()
    print res

def main():
    # url = "http://testpay.baziline.ws/pay/iran_callback"
    # data = {'orderId':'259','extInfo':'3002', 'productNum':'1'}

    url = "https://pardakht.cafebazaar.ir/devapi/v2/auth/token/"
    data = {'grant_type':'authorization_code','code':'t52ctD4x8nYAb7bDz2N4ZIEKJ8gP9a', 'client_id':'GdbXiSirsUWRi32nkknTrpgPzMUnUXQuLn36L9lp','client_secret':'o2l9247svkeFm9Gt9QpdOpaosidizIIckY0gLOeGK8yt2SAmCwQAm5uaJmsb','redirect_uri':'http://91.98.39.60'}
    httpPost(url, data)

main()
