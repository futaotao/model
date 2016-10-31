using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace model
{
    class Program
    {
        static void Main(string[] args)
        {
            //String path = "C:\\Users\\Administrator\\Desktop\\xm\\10.txt";

            String path = "C:\\Users\\Administrator\\Desktop\\test\\19.txt";



            //解析并保存
            String result = readFile(path);

            //保存错误行
            String savePath = "C:\\Users\\Administrator\\Desktop\\test\\r.txt";
            saveFile(savePath, result);

        }

        private static String readFile(String filePath)
        {
            if (!File.Exists(filePath))
            {
                return "";
            }
            StreamReader sr = new StreamReader(filePath);

            String result = "";

            string str = "";
            int i = 0;
            while ((str = sr.ReadLine()) != null)
            {
                i = i + 1;
                //解析 str
                string[] info = split(str,'|');
                bool isRight = false;
                if (info != null && info.Length == 8)
                {
                    string fingerprint = info[5];
                    isRight = isRightFingerprint(fingerprint);
                }

                if (!isRight)
                {
                    //格式不正确 
                    result += "Line " + i + "; " + isRight + "\n";
                }
                else
                {
                    //格式解析
                    Dictionary<String, String> dic = analyze(str);
                    String content = "";
                    content += "ro.build.id=" + dic["ro.build.id"] + "\n";
                    content += "ro.build.display.id=" + dic["ro.build.display.id"] + "\n";
                    content += "ro.build.version.incremental=" + dic["ro.build.version.incremental"] + "\n";
                    content += "ro.build.version.sdk=" + dic["ro.build.version.sdk"] + "\n";
                    content += "ro.build.version.codename=" + dic["ro.build.version.codename"] + "\n";
                    content += "ro.build.version.release=" + dic["ro.build.version.release"] + "\n";
                    content += "ro.build.type=" + dic["ro.build.type"] + "\n";
                    content += "ro.build.user=" + dic["ro.build.user"] + "\n";
                    content += "ro.build.tags=" + dic["ro.build.tags"] + "\n";
                    content += "ro.build.product=" + dic["ro.build.product"] + "\n";
                    content += "ro.product.model=" + dic["ro.product.model"] + "\n";
                    content += "ro.product.name=" + dic["ro.product.name"] + "\n";
                    content += "ro.product.device=" + dic["ro.product.device"] + "\n";
                    content += "ro.product.brand=" + dic["ro.product.brand"] + "\n";
                    content += "ro.product.manufacturer=" + dic["ro.product.manufacturer"] + "\n";
                    content += "ro.product.board=" + dic["ro.product.board"] + "\n";
                    content += "ro.build.description=" + dic["ro.build.description"] + "\n";
                    content += "ro.build.fingerprint=" + dic["ro.build.fingerprint"] + "\n";
                    content += "sdk_version=" + dic["sdk_version"] + "\n";
                    content += "screen_x_y=" + dic["screen_x_y"];

                    // 10  10_
                    // 15  15_
                    // 21  21_
                    // 22  22_
                    //String fileName = "10_" + dic["ro.product.brand"] + "_" + dic["sdk_version"] + "_" + i;
                    String fileName = dic["ro.product.brand"] + "_" + dic["sdk_version"] + "_" + i;

                    String newPath = filePath.Substring(0, filePath.LastIndexOf('\\')) + "\\" + dic["sdk_version"] + "\\" + fileName;
                    saveFile(newPath, content);
                }

                //if (info != null && info.Length == 8)
                //{
                //    String screen = info[3];
                //    String newScreen = generateScreen(screen);

                //    result += newScreen + "\n";
                //}

            }
            sr.Close();
            return result;

            //StreamWriter writer = new StreamWriter(filePath);
            //writer.Write("");
            //writer.Flush();
            //writer.Close();
        }

        /// <summary>
        /// 保存内容到指定文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        private static void saveFile(String filePath, String content) {

            String dir = filePath.Substring(0, filePath.LastIndexOf('\\'));
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(filePath))
            {
               FileStream fs =  File.Create(filePath);
               fs.Close();
            }
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 0 sdk
        /// 1 model
        /// 2 brand
        /// 3 screen
        /// 4 product
        /// 5 fingerprint
        /// 6 release
        /// 7 manufacturer
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        /// <summary>
        /// 0 brand
        /// 1 name
        /// 2 device:release
        /// 3 id
        /// 4 incremental:type
        /// 5 tags
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <returns></returns>

        //按api版本来处理 
        // 10 15 --> 16
        // 21 22 -- > 18
        private static Dictionary<String, String> analyze(String str) {
            string[] info = split(str, '|');

            int sdk = int.Parse(info[0]);
            
            if (sdk == 10 || sdk == 15) {
                sdk = 16;
            }

            if (sdk == 21 || sdk == 22) {
                sdk = 18;
            }

            String model = info[1];
            String brand = info[2];
            String screen = info[3];
            screen = generateScreen(screen);
            //19版本特殊分辨率   按键精灵
            if (sdk == 19) {
                screen = "720x1280";
            }
            String product = info[4];
            String fingerprint = info[5];
            String release = info[6];
            String manufacturer = info[7];

            
            string[] finger = split(fingerprint, '/');
            if (string.IsNullOrEmpty(brand)) {
                brand = finger[0];
            }

            String name = finger[1];
            String deviceANDrelease = finger[2];
            string[] split1 = split(deviceANDrelease, ':');
            String device = split1[0];
            if (string.IsNullOrEmpty(release)) {
                release = split1[1];
            }

            if (sdk == 16) {
                release = "4.1.1";
            }else if(sdk == 17){
                release = "4.2.2";
            }
            else if (sdk == 18) {
                release = "4.3";
            }
            else if (sdk == 19) {
                release = "4.4.4";
            }

            
            String id = finger[3];
            String incrementalANDtype = finger[4];
            string[] split2 = split(incrementalANDtype, ':');
            String incremental = split2[0];
           
            String type = split2[1];
            if (type.Contains("n"))
            {
                type = "eng";
            }
            else {
                type = "user";
            }
            

            String tags = finger[5];
            if (tags.Contains("r"))
            {
                tags = "release-keys";
            }
            else {
                tags = "test-keys";
            }

            
            String board = "";
            String codename = "REL";
            String user = "builder";

            String description = name + "-" + type + " " + release + " " + id + " " + incremental + " " + tags;
            fingerprint = brand + "/" + name + "/" + device + ":" + release + "/" + id + "/" + incremental + ":" + type + "/" + tags;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ro.build.id",id);
            dic.Add("ro.build.display.id",id);
            dic.Add("ro.build.version.incremental",incremental);
            dic.Add("ro.build.version.sdk", sdk+"");
            dic.Add("ro.build.version.codename",codename);
            dic.Add("ro.build.version.release", release);
            dic.Add("ro.build.type",type);
            dic.Add("ro.build.user",user);
            dic.Add("ro.build.tags",tags);
            dic.Add("ro.build.product", product);
            dic.Add("ro.product.model", model);
            dic.Add("ro.product.name", name);
            dic.Add("ro.product.device", device);
            dic.Add("ro.product.brand", brand);
            dic.Add("ro.product.manufacturer", manufacturer);
            dic.Add("ro.product.board", board);
            dic.Add("ro.build.description", description);
            dic.Add("ro.build.fingerprint", fingerprint);

            dic.Add("sdk_version", sdk + "");
            dic.Add("screen_x_y", screen);
            return dic;
        }



        /// <summary>
        /// 判断fingerprint 合法性
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <returns></returns>
        private static Boolean isRightFingerprint(String fingerprint) {
            string[] array = split(fingerprint,'/');
            if (array != null && array.Length == 6) {

                String brand = array[0];
                String name = array[1];
                String deviceANDrelease = array[2];
                String id = array[3];
                String incrementalANDtype = array[4];
                String tags = array[5];

                String device = "";
                String release = "";
                string[] split1 = split(deviceANDrelease, ':');
                if (split1 != null && split1.Length == 2)
                {
                    device = split1[0];
                    release = split1[1];
                }

                String incremental = "";
                String type = "";

                string[] split2 = split(incrementalANDtype, ':');
                if (split2 != null && split2.Length == 2) {
                    incremental = split2[0];
                    type = split2[1];
                }

                if (String.IsNullOrEmpty(brand) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(device) || String.IsNullOrEmpty(release)
                    || String.IsNullOrEmpty(id) || String.IsNullOrEmpty(incremental) || String.IsNullOrEmpty(type) || String.IsNullOrEmpty(tags))
                {
                    return false;
                }

                return true;
                


            }
            return false;
        }


        /// <summary>
        /// 根据老的screen 生成新的 screen
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        private static String generateScreen(String screen) {
            string[] s = split(screen, '-');
            if (s == null || s.Length != 2) {
                return "-1";
            }
            int screenX =  int.Parse(s[0]);
            int screenY = int.Parse(s[1]);
            int temp = -1;

            if (screenX > screenY) {
                temp = screenX;
                screenX = screenY;
                screenY = temp;
            }

            //生成新的
            if (screenX < 480) {
                //
                screenX = 480;
                screenY = 800;
            }
            else if (screenX < 720)
            {
               // x = 480
                screenX = 480;
                if (screenY != 800) {
                    screenY = 854;
                }
            }
            else if (screenX < 1080)
            {
                // x = 720
                screenX = 720;
                screenY = 1280;
            }
            else {
                // x = 1080
                screenX = 1080;
                screenY = 1920;
            }

            return screenX + "x" + screenY;

        }

        private static string[] split(String str, Char chr) {
            return str.Split(chr);
        }
    }
}
