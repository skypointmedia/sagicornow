using System;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace SagicorNow
{
	public class Form1 : Form
	{

		private void button1_Click(object sender, EventArgs e)
		{
			var client = new AltNewBusinessService.NewBusinessClient("WSHttpBinding_NewBusiness");
			var txLife = Create103();
			var request = FromEntityToByte(txLife);
			byte[] resp = client.SubmitNewBusinessApplication(request);
			var response = FromByteToEntity<NewBusinessService.TXLife>(resp);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var client = new NewBusinessService.NewBusinessClient("CustomBinding_NewBusiness");
			var txLife = Create103();
			NewBusinessService.TXLife response = client.SubmitNewBusinessApplication(txLife);
		}

		private NewBusinessService.TXLife Create103()
		{
			var txLife = new NewBusinessService.TXLife();
			txLife.TXLifeRequest = new NewBusinessService.TXLifeRequest[1];
			txLife.TXLifeRequest[0] = new NewBusinessService.TXLifeRequest();

			return txLife;
		}

		private Byte[] FromEntityToByte<T>(T entity) where T : class
		{
			if (entity != null)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					new XmlSerializer(typeof(T)).Serialize(stream, entity);
					stream.Flush();
					return stream.ToArray();
				}
			}
			return new Byte[0];
		}

		private T FromByteToEntity<T>(Byte[] data) where T : class, new()
		{
			if (data != null && data.Length > 0)
			{
				using (MemoryStream stream = new MemoryStream(data))
				{
					return new XmlSerializer(typeof(T)).Deserialize(stream) as T;
				}
			}
			return new T();
		}
	}
}
