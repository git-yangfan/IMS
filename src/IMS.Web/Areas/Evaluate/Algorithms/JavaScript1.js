function Compare() {
    $.post("/Evaluate/Evaluate/CompareDevice", { deviceA: "设备1", deviceB: "设备7" },
       function (Data) {
           json = $.parseJSON(Data);
           if (json.flag) {
               var tdfortr2 = [];
               var tdfortr3 = [];
               var tdfortr4 = [];
               var tdfortr5 = [];
               var tdfortr6 = [];
               var tdfortr7 = [];
               var tdfortr8 = [];
               var tdfortr8 = []; var tdfortr9 = [];
               var tdfortr10 = [];
               var tdfortr11 = [];
               var tdfortr12 = [];
               var tdfortr13 = [];
               var tdfortr14 = [];
               var tdfortr15 = [];
               var tdfortr16 = [];
               var tdfortr17 = [];
               var tdfortr18 = [];
               var tdfortr19 = [];
               var tdfortr20 = [];
               var tdfortr21 = [];
               tdfortr3.push('<th>设备简称</th>');
               tdfortr4.push('<th>设备名称</th>');
               tdfortr5.push('<th>设备编号</th>');
               tdfortr6.push('<th>生产厂家</th>');
               tdfortr7.push('<th>出厂日期</th>');
               tdfortr8.push('<th>设备规格</th>');
               tdfortr9.push('<th>设备类型</th>');
               tdfortr10.push('<th>投产日期</th>');
               tdfortr11.push('<th>使用部门</th>');
               tdfortr12.push('<th>所属车间</th>');
               tdfortr13.push('<th>日历时间</th>');
               tdfortr14.push('<th>完好时间</th>');
               tdfortr15.push('<th>开机时间</th>');
               tdfortr16.push('<th>开机率</th>');
               tdfortr17.push('<th>MTBF</th>');
               tdfortr18.push('<th>ALPH</th>');
               tdfortr19.push('<th>BETA</th>');
               tdfortr20.push('<th>概率密度函数图</th>');
               tdfortr21.push('<th>硬盘容量</th>');
               for (var i = 0; i < json.data.length; i++) {
                   tdfortr2.push('<td><div class="select-item"><span class="select-trigger">' + json.data[i].Device.ShortName + '</span></div><div class="select-item"><span class="select-trigger">联想E40-70-looogIFI（i5 4210U/4GB/500GB/Win7)</span></div></td>');
                 
                   tdfortr3.push('<td><div class="product-name-inner"><a href="#">' + json.data[i].Device.ShortName + '</a></div></td>');
                   tdfortr4.push('<td><a href="#" target="_blank">' + json.data[i].Device.ShortName + '</a></td>');
                   tdfortr5.push('<td> <div class="grade clearfix"><span class="score">' + json.data[i].Device.ShortName + '</span></td>');
                   tdfortr6.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr7.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr8.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr9.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr10.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr11.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr12.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr13.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr14.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr15.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr16.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr17.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr18.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr19.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr20.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                   tdfortr21.push('<td><div class="param-content">' + json.data[i].Device.ShortName + '</div></td>');
                 
                   
               }
               tdfortr3.push('<td class="unavailable"></td>');
               tdfortr4.push('<td class="unavailable"></td>');
               tdfortr5.push('<td class="unavailable"></td>');
               tdfortr6.push('<td class="unavailable"></td>');
               tdfortr7.push('<td class="unavailable"></td>');
               tdfortr8.push('<td class="unavailable"></td>');
               tdfortr9.push('<td class="unavailable"></td>');
               tdfortr10.push('<td class="unavailable"></td>');
               tdfortr11.push('<td class="unavailable"></td>');
               tdfortr12.push('<td class="unavailable"></td>');
               tdfortr13.push('<td class="unavailable"></td>');
               tdfortr14.push('<td class="unavailable"></td>');
               tdfortr15.push('<td class="unavailable"></td>');
               tdfortr16.push('<td class="unavailable"></td>');
               tdfortr17.push('<td class="unavailable"></td>');
               tdfortr18.push('<td class="unavailable"></td>');
               tdfortr19.push('<td class="unavailable"></td>');
               tdfortr20.push('<td class="unavailable"></td>');
               tdfortr21.push('<td class="unavailable"></td>');

               $(".product-choose").html(tdfortr2);
               $("#tr3").html(tdfortr3);
               $("#tr4").html(tdfortr4);
               $("#tr5").html(tdfortr5);
               $("#tr6").html(tdfortr6);
               $("#tr7").html(tdfortr7);
               $("#tr8").html(tdfortr8);
               $("#tr9").html(tdfortr9);
               $("#tr10").html(tdfortr10);
               $("#tr11").html(tdfortr11);
               $("#tr12").html(tdfortr12);
               $("#tr13").html(tdfortr13);
               $("#tr14").html(tdfortr14);
               $("#tr15").html(tdfortr15);
               $("#tr16").html(tdfortr16);
               $("#tr17").html(tdfortr17);
               $("#tr18").html(tdfortr18);
               $("#tr19").html(tdfortr19);
               $("#tr20").html(tdfortr20);
               $("#tr21").html(tdfortr21);
            

           }
       });

}