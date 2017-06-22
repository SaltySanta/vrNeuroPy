using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

/// <summary>
/// Holds data of a config and makes it possible to load a config from a XML-File
/// @details
/// - Created on: November 2011
/// - Author: Marcel Richter 
/// </summary>
/// \ingroup UnityVRClasses
[Serializable ]
public class cConfiguration
{
    #region Members
    

    /// <summary>
    /// access the port
    /// </summary>
    public int LocalPort
    {
        get; private set; 
    }
  
    public int IPAddressEntry
    {
        get; private set;
    }

    /// <summary>
    /// access the IP
    /// </summary>
    /*
	public string LocalIP
    {
        get; private set;
    }*/		
	
	public bool  SyncMode
	{
		get; private set;
	}
		
	public float SimulationTimePerFrame
	{
		get; private set;
	}
	
	public int ImageResolutionWidth
	{
		get; private set;
	}	
		
	public bool SendGridPosition
	{
		get; private set;
	}
	
	public float MovementSpeed
	{
		get; private set;
	}
	
	public int CameraDisplayWidth
	{
		get; private set;
	}
			
	public int FovHorizontal
	{
		get; private set;
	}
	
	public int FovVertical
	{
		get; private set;
	}
	
    #endregion

    #region Constructor

    /// <summary>
    /// Create from XMLFile at Path
    /// </summary>
    /// <param name="Path">Path to file</param>
    public cConfiguration(string Path)
    {
        LoadFromFile(Path);
    }

    /// <summary>
    /// Loads Data from XML-File
    /// </summary>
    /// <param name="Path">Path to file</param>
    private void LoadFromFile(string Path)
    {
        cConfiguration Buffer = new cConfiguration();
        Buffer = (cConfiguration)ReadObjectFromXmlFile(Path, typeof(cConfiguration));
        //this.LocalIP = Buffer.LocalIP;
        this.LocalPort = Buffer.LocalPort;
		this.SyncMode = Buffer.SyncMode;
		this.SimulationTimePerFrame = Buffer.SimulationTimePerFrame;
		this.ImageResolutionWidth = Buffer.ImageResolutionWidth;
		this.SendGridPosition = Buffer.SendGridPosition;
		this.MovementSpeed = Buffer.MovementSpeed;
		this.CameraDisplayWidth = Buffer.CameraDisplayWidth;
		this.FovHorizontal = Buffer.FovHorizontal;
		this.FovVertical = Buffer.FovVertical;
		this.IPAddressEntry = Buffer.IPAddressEntry;
    }

    /// <summary>
    /// create standardconfiguration
    /// </summary>
    public cConfiguration()
    {
        this.LocalPort = 1337;
        //this.LocalIP = "134.109.204.12";
		this.SyncMode = false;
		
		this.SimulationTimePerFrame = 0.1f;
		this.ImageResolutionWidth = 512;
		this.SendGridPosition = false;
		this.MovementSpeed = 10.0f;	
		this.CameraDisplayWidth = 256;
		this.FovHorizontal = 120;
		this.FovVertical = 90;
		this.IPAddressEntry = 1;
		//WriteObjectToXmlFile("appconfigNew.config_", typeof(cConfiguration), this, false, true); // <- write out a sample file

    }
    #endregion

    #region CheckFileExistence
    /// <summary>
    /// CheckExistence
    /// </summary>
    /// <param name="filename">Path</param>
    /// <returns>true if exists </returns>
    public static bool CheckFileExistence(string filename)
    {
        System.IO.FileInfo FI = new FileInfo(filename);
        return FI.Exists;
    }
    #endregion

    #region ReadObjectFromXmlFile
    /// <summary>
    /// Reads a Object from a XML-File
    /// </summary>
    /// <param name="filename">path to file</param>
    /// <param name="type">typeof() of Object to load</param>
    /// <returns>Loaded Oject</returns>
    public static object ReadObjectFromXmlFile(string filename, System.Type type)
    {
        //Objektinstanziierung
        object o = new object();

        if (CheckFileExistence(filename) != true)
            throw new Exception("APPConfig.config not found! That file is needed to initialize the Network. Look in the 'Instal'-folder!");
        //WriteObjectToXmlFile(filename, type, obj, Flag_UsingSOAP, true);


        //Create IOStream to File
        System.IO.FileStream FS = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);

        //Create XML-Serializer
        XmlSerializer XS = new XmlSerializer(type);

        //Load Data
        o = XS.Deserialize(FS);

        //Close File
        FS.Close();

        return o;
    }
    #endregion

    #region WriteObjectToXmlFile
    /// <summary>
    /// Saves Object in XML-File
    /// </summary>
    /// <param name="filename">Path to File</param>
    /// <param name="type">type of object</param>
    /// <param name="data">objekt</param>
    /// <param name="Flag_UsingSOAP"></param>
    /// <param name="Flag_Overwrite">Ovewrite if existing?</param>
    /// <returns>true if data was written to HDD</returns>
    public static bool WriteObjectToXmlFile(string filename, System.Type type, object data, bool Flag_UsingSOAP, bool Flag_Overwrite)
    {

        bool fileex = CheckFileExistence(filename);
        if (Flag_Overwrite || !fileex)
        {

            FileStream FS = new FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);

            XmlSerializer XS = new XmlSerializer(type);

            XS.Serialize(FS, data);

            FS.Close();
        }
        else
        {
            return false;
        }
        return true;
    }

    #endregion
}

