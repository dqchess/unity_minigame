<?php
header("Content-type: text/html; charset=utf-8");
include_once('CrazyImageDB.php');
include_once('WordItemInfo.php');
include_once('../Common/Download.php');
include_once('../Common/Common.php');
include_once('../Common/Html/simple_html_dom.php');

//疯狂猜图答案_疯狂猜图所有答案_疯狂猜图品牌__72G疯狂猜图专区
//http://fkct.72g.com/

function get_html($url)
{
    $html = new simple_html_dom();

    // // 从url中加载  
    // $html->load_file('http://www.jb51.net');  

    // // 从字符串中加载  
    // $html->load('<html><body>从字符串中加载html文档演示</body></html>');  

    //从文件中加载  
    $html->load_file($url);

    return $html;
}

// 疯狂猜图  http://www.caichengyu.com/fkct/
class ParseWord //extends Thread
{

    public $url;
    public $id;
    public $channel;
    public $listItem;
    public $listSort = array();
    public  $page_total = 10; //10 
    public $ROOT_SAVE_DIR = "Data";
    public $WEB_HOME = "http://www.iciba.com";
    public $IMAGE_DIR = "Word";
    public $PIC_DIR = "Pic";
    public function DoPaser()
    {
        $save_dir = $this->ROOT_SAVE_DIR;
        if (!is_dir($save_dir)) {
            mkdir($save_dir);
        }

        $save_dir = $this->ROOT_SAVE_DIR . "/" . $this->IMAGE_DIR;
        if (!is_dir($save_dir)) {
            mkdir($save_dir);
        }
        $word = "word";
        $info =  $this->PaserWordInfo($this->WEB_HOME . "/" . $word);
        $info->title = $word;
        $this->SaveWordJson($save_dir, $info);
    }



    public function SaveWordJson($save_dir, $info)
    {
        //save sort
        $savefilepath = $save_dir . "/" . $info->title . ".json";
        $ret = file_exists($savefilepath);
        if ($ret) {
            // return;
        } {



            $element = array(
                'title' => $info->title,
                'translation' => $info->translation,
                'change' => $info->change,

            );
            //JSON_UNESCAPED_SLASHES json去除反斜杠 JSON_UNESCAPED_UNICODE中文不用\u格式
            $jsn = urldecode(json_encode($element, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE));

            // "[  ]"
            //$jsn = str_replace("\"[", "[", $jsn);
            //$jsn = str_replace("]\"", "]", $jsn);

            $fp = fopen($savefilepath, "w");
            if (!$fp) {
                echo "打开文件失败<br>";
                return;
            }
            $flag = fwrite($fp, $jsn);
            if (!$flag) {
                echo "写入文件失败<br>";
            }
            fclose($fp);
        }
    }


    //http://www.iciba.com/word
    function PaserWordInfo($url)
    {
        $info = new WordItemInfo();
        $info->translation = array();

        $html = get_html($url);
        if (!$html) {
            echo "PaserSortList html fail\n";
            return $info;
        }
        $ul_main = $html->find('ul[class=base-list switch_part]', 0);
        if (!$ul_main) {
            echo "PaserWordInfo find ul_main fail\n";
            return $info;
        }
        $arry_li = $ul_main->find('li');
        foreach ($arry_li as $li) {
            $arry_span = $li->find('span');
            $str = "";
            foreach ($arry_span as $span) {
                if ($span->class == "prop") { }
                $str = $str . " " . $span->plaintext;
            }

            array_push($info->translation, $str);
        } {
            //变形 change clearfix
            $change_li = $html->find('li[class=change clearfix]', 0);
            $arry_span = $change_li->find('span');
            $str = "";
            foreach ($arry_span as $span) {
                $strtmp =  $span->plaintext;
                //网页空格
                $strtmp = str_replace("	", "", $strtmp);
                //普通空格
                $strtmp = str_replace(" ", "", $strtmp);

                $str = $str . " " . $strtmp;
            }
            $info->change = $str;
        }

        return $info;
    }
}


$parser = new ParseWord();
$parser->DoPaser();

echo 'done<br>';