using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encrpyData;//是否加密数据的标志
    private string codeWord = "RPG#2026_SaveKey!";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)//构造函数，传入数据文件夹路径和数据文件名称
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encrpyData = encryptData;
    }

    public void SaveData(GameData gameData)
    {
        try
        {
            //1.确保数据文件夹存在，如果不存在则创建
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //2.将游戏数据转换为JSON格式字符串
            string dataToSave = JsonUtility.ToJson(gameData, true);

            if (encrpyData)
                dataToSave = EncryptDecrypt(dataToSave);//如果加密数据标志为true，则对JSON字符串进行加密处理

            //3.使用FileStream和StreamWriter将JSON字符串写入数据文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                //4.使用StreamWriter将JSON字符串写入文件
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave);//将JSON字符串写入文件
                }
            }
        }

        catch (Exception e)
        {
            //如果在保存数据过程中发生任何异常，捕获并记录错误信息
            Debug.LogError("Error saving data to file: " + fullPath + "\n" + e);
        }
    }

    public GameData LoadData()
    {
        GameData loadData = null;

        //1.检查数据文件是否存在，如果存在则继续加载数据，否则返回null
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                //2.使用FileStream和StreamReader读取数据文件中的JSON字符串
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    //3.使用StreamReader读取文件内容并将其存储在dataToLoad变量中
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encrpyData)
                    dataToLoad = EncryptDecrypt(dataToLoad);//如果加密数据标志为true，则对读取到的JSON字符串进行解密处理

                //4.将JSON字符串转换回GameData对象并返回
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            catch (Exception e)
            {
                //如果在加载数据过程中发生任何异常，捕获并记录错误信息
                Debug.LogError("Error loading data from file: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }

    public void Delete()
    {
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        string modifedData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);//使用异或运算对数据进行加密或解密
        }

        return modifedData;
    }
}
