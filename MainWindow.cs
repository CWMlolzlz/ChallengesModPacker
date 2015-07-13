using System;
using Gtk;
using System.IO;

public partial class MainWindow: Gtk.Window
{

	bool hasXMLFile = false;
	private static readonly string DIRECTORY_UNDER_LOCAL_APPDATA = @"\Colossal Order\Cities_Skylines\Addons\Mods";

	public MainWindow () : base (Gtk.WindowType.Toplevel){
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a){
		Gtk.Application.Quit ();
		a.RetVal = true;
	}

	protected void OnOpenFile (object sender, EventArgs e){		
		Gtk.FileChooserDialog filechooser =
			new Gtk.FileChooserDialog("Choose the file to open",
				this,
				FileChooserAction.Open,
				"Cancel",ResponseType.Cancel,
				"Open",ResponseType.Accept);

		if (filechooser.Run() == (int)ResponseType.Accept){	
			this.XMLTextView.Buffer.Text = "";
			foreach(string line in File.ReadLines(filechooser.Filename)){
				this.XMLTextView.Buffer.Text += line + "\n";
				System.Diagnostics.Debug.WriteLine(line);
			}
			hasXMLFile = true;
			//System.IO.FileStream file = System.IO.File.OpenRead(filechooser.Filename);
			//System.Diagnostics.Debug.WriteLine(file.);
			//file.Close();
		}

		filechooser.Destroy();
	}

	protected void OnExport (object o, EventArgs e){
		if (this.ModNameField.Text != "" && this.ModDescriptionField.Text != "" && hasXMLFile) {
			try{
				string name = this.ModNameField.Text;
				string desc = this.ModDescriptionField.Text;
				//do export
				//create folder in addons\\mods
				string modFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + DIRECTORY_UNDER_LOCAL_APPDATA + @"\" + name;
				string csFileContents = CS_FILE_START + name + CS_FILE_MIDDLE1 + name + CS_FILE_MIDDLE2 + desc + CS_FILE_END;

				System.Diagnostics.Debug.WriteLine ("Mod folder: " + modFolder.ToString ());
				System.Diagnostics.Debug.WriteLine("CS File Contents: " + csFileContents);

				Directory.CreateDirectory (modFolder + @"\Source");
				Directory.CreateDirectory (modFolder + @"\Challenges");
				System.IO.File.WriteAllText (modFolder + @"\Source\GeneratedChallengeMod.cs", csFileContents);
				System.IO.File.WriteAllText (modFolder + @"\Challenges\"+name+@".xml", this.XMLTextView.Buffer.Text);
			}catch(System.ArgumentException exception){
				ThrowErrorDialog("Invalid characters used in either mod name or mod description");
			}
						

			return;
		}

		if (!hasXMLFile) {
			ThrowErrorDialog ("An XML file has not been selected");
		} 

		if(this.ModNameField.Text == ""){
			ThrowErrorDialog ("Mod name has not been filled in");
		}

		if(this.ModDescriptionField.Text == ""){
			ThrowErrorDialog ("Mod description has not been filled in");
		}


		//throw new NotImplementedException ();
	}

	private void ThrowErrorDialog(string msg){
		Gtk.MessageDialog errorDialog = new MessageDialog (this, 
			DialogFlags.DestroyWithParent, MessageType.Error, 
			ButtonsType.Close, msg);
		errorDialog.Run();
		errorDialog.Destroy ();
	}

	protected void OnURLCLick (object o, EventArgs e){
		System.Diagnostics.Process.Start(((Gtk.Button)o).Label.ToString());
		//throw new NotImplementedException ();
	}

	private static readonly string CS_FILE_START = "using System; using ICities;using UnityEngine;namespace "; 

	private static readonly string CS_FILE_MIDDLE1 = "{public class ChallangeMod : IUserMod{public string Name{get{return \"";
	private static readonly string CS_FILE_MIDDLE2 = "\";}}public string Description{get {return \"";
	private static readonly string CS_FILE_END = "\";}}}}";


}