(this.webpackJsonpweb3gl=this.webpackJsonpweb3gl||[]).push([[20],{269:function(n,e,t){},282:function(n,e){},292:function(n,e){},294:function(n,e){},303:function(n,e){},305:function(n,e){},330:function(n,e){},332:function(n,e){},333:function(n,e){},338:function(n,e){},340:function(n,e){},347:function(n,e){},349:function(n,e){},361:function(n,e){},363:function(n,e){},375:function(n,e){},378:function(n,e){},381:function(n,e){},431:function(n,e){},437:function(n,e){},535:function(n,e,t){"use strict";t.r(e);var o,r=t(81),c=t.n(r),a=t(146),s=t.n(a),i=(t(269),t(76)),u=t(2),w=t.n(u),f=t(38),l=t(235),p=t(240),d=t.n(p);window.web3gl={connect:b,connectAccount:"",sendContract:function(n,e,t,o,r,c){return x.apply(this,arguments)},sendContractResponse:"",sendTransaction:function(n,e,t){return k.apply(this,arguments)},sendTransactionResponse:"",signMessage:function(n){return m.apply(this,arguments)},signMessageResponse:""};var g=!0,h=Object(l.a)({networkName:window.web3NetworkName,networkId:window.web3NetworkId,subscriptions:{address:function(){g||(window.location.reload(),b())},wallet:function(n){o=new d.a(n.provider)},network:function(){g||(window.location.reload(),b())}},walletSelect:{wallets:[{walletName:"metamask",preferred:!0},{walletName:"walletConnect",infuraKey:"2d0062a43e9e4086829df115488b45a8",preferred:!0},{walletName:"torus",preferred:!0}]}});function b(){return v.apply(this,arguments)}function v(){return(v=Object(f.a)(w.a.mark((function n(){return w.a.wrap((function(n){for(;;)switch(n.prev=n.next){case 0:return n.prev=0,n.next=3,h.walletSelect();case 3:return n.next=5,h.walletCheck();case 5:return g=!1,n.next=8,o.eth.net.getId();case 8:if(n.t0=n.sent,n.t1=window.web3NetworkId,n.t0!==n.t1){n.next=14;break}return n.next=13,o.eth.getAccounts();case 13:window.web3gl.connectAccount=n.sent[0];case 14:n.next=19;break;case 16:n.prev=16,n.t2=n.catch(0),console.log(n.t2);case 19:case"end":return n.stop()}}),n,null,[[0,16]])})))).apply(this,arguments)}function m(){return(m=Object(f.a)(w.a.mark((function n(e){var t,r;return w.a.wrap((function(n){for(;;)switch(n.prev=n.next){case 0:return n.prev=0,n.next=3,o.eth.getAccounts();case 3:return t=n.sent[0],n.next=6,o.eth.personal.sign(e,t,"");case 6:r=n.sent,window.web3gl.signMessageResponse=r,n.next=13;break;case 10:n.prev=10,n.t0=n.catch(0),window.web3gl.signMessageResponse=n.t0.message;case 13:case"end":return n.stop()}}),n,null,[[0,10]])})))).apply(this,arguments)}function x(){return(x=Object(f.a)(w.a.mark((function n(e,t,r,c,a,s){var u,f;return w.a.wrap((function(n){for(;;)switch(n.prev=n.next){case 0:return n.next=2,o.eth.getAccounts();case 2:f=n.sent[0],(u=new o.eth.Contract(JSON.parse(t),r).methods)[e].apply(u,Object(i.a)(JSON.parse(c))).send({from:f,value:a,gas:s||void 0}).on("transactionHash",(function(n){window.web3gl.sendContractResponse=n})).on("error",(function(n){window.web3gl.sendContractResponse=n.message}));case 4:case"end":return n.stop()}}),n)})))).apply(this,arguments)}function k(){return(k=Object(f.a)(w.a.mark((function n(e,t,r){var c;return w.a.wrap((function(n){for(;;)switch(n.prev=n.next){case 0:return n.next=2,o.eth.getAccounts();case 2:c=n.sent[0],o.eth.sendTransaction({from:c,to:e,value:t,gas:r||void 0}).on("transactionHash",(function(n){window.web3gl.sendTransactionResponse=n})).on("error",(function(n){window.web3gl.sendTransactionResponse=n.message}));case 4:case"end":return n.stop()}}),n)})))).apply(this,arguments)}var j=t(79);var y=function(){return Object(j.jsx)("div",{})},O=function(n){n&&n instanceof Function&&t.e(79).then(t.bind(null,1794)).then((function(e){var t=e.getCLS,o=e.getFID,r=e.getFCP,c=e.getLCP,a=e.getTTFB;t(n),o(n),r(n),c(n),a(n)}))};s.a.render(Object(j.jsx)(c.a.StrictMode,{children:Object(j.jsx)(y,{})}),document.getElementById("root")),O()}},[[535,21,22]]]);