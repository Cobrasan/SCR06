using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Alchemist
{
    public class BankXmlAccessor : XMLAccessor
    {
        /// <summary>
        /// bankdata.xmlのスキーマ定義
        /// </summary>
        private const string schema = @"
<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
	<!-- bank_number型 -->
	<xsd:simpleType name='bank_number'>
		<xsd:restriction base='xsd:integer'>
			<xsd:minInclusive value='0' />
		</xsd:restriction>
	</xsd:simpleType>
	
	<!-- address_string -->
	<xsd:simpleType name='address_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:pattern value='^0x[0-9A-F]{4}'/>
		</xsd:restriction>
	</xsd:simpleType>	

	<!-- workid_type -->
	<xsd:simpleType name='type_number'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='3' />
			<xsd:enumeration value='4' />
			<xsd:enumeration value='5' />
		</xsd:restriction>
	</xsd:simpleType>	

	<!-- value_number -->
	<xsd:simpleType name='value_number'>
        <xsd:union>
            <xsd:simpleType>
                <xsd:restriction base='xsd:double' />
            </xsd:simpleType>
            <xsd:simpleType>
                <xsd:restriction base='xsd:string' />
            </xsd:simpleType>
        </xsd:union>
	</xsd:simpleType>

	<!-- bankdata要素定義 -->
	<xsd:element name='bankdata' type='bankdata_type'>
		<!-- 主キー定義 -->
		<xsd:unique name='pkBank'>
			<xsd:selector xpath='bank' />
			<xsd:field xpath='@no' />
		</xsd:unique>
	</xsd:element>

	<xsd:complexType name='bankdata_type'>
        <xsd:sequence>
		    <xsd:element name='bank' minOccurs='1' maxOccurs='unbounded' type='bank_type'>
				<!-- 主キー定義 -->
				<xsd:unique name='pkEntry'>
					<xsd:selector xpath='entry' />
					<xsd:field xpath='@type' />
					<xsd:field xpath='@workid' />
				</xsd:unique>
			</xsd:element>
        </xsd:sequence>

		<!-- selectedno属性定義 -->
		<xsd:attribute name='selectedno' use='required' type='bank_number' />
	</xsd:complexType>


	<!-- bank要素定義 -->
	<xsd:complexType name='bank_type'>
		<xsd:sequence>
			<xsd:element name='entry' minOccurs='0' maxOccurs='unbounded' type='entry_type' />
		</xsd:sequence>
		
		<!-- no属性定義 -->
		<xsd:attribute name='no' use='required' type='bank_number' />
		
		<!-- comment属性定義 -->
		<xsd:attribute name='comment' use='required' type='xsd:string' />

        <!-- wiretype属性定義 -->
		<xsd:attribute name='wirename' use='required' type='xsd:string' />

        <!-- wirelength属性定義 -->
		<xsd:attribute name='wirelength' use='required' type='xsd:string' />

        <!-- strip1属性定義 -->
		<xsd:attribute name='strip1' use='required' type='xsd:string' />

        <!-- strip2属性定義 -->
		<xsd:attribute name='strip2' use='required' type='xsd:string' />
    </xsd:complexType>

	<!-- entry要素定義 -->
	<xsd:complexType name='entry_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- type属性 -->
		<xsd:attribute name='type' use='required' type='type_number' />
			
		<!-- workid属性 -->
		<xsd:attribute name='workid' use='required' type='address_string' />
			
		<!-- value属性 -->
		<xsd:attribute name='value' use='required' type='value_number' />
	</xsd:complexType>
</xsd:schema>
";
        protected override string GetSchema()
        {
            return schema;
        }

        public override void NewDocument()
        {
            doc = new XDocument(
                new XElement("bankdata", new XElement("bank"))
            );

            doc.Element("bankdata").SetAttributeValue("selectedno", 0);
            doc.Element("bankdata").Element("bank").SetAttributeValue("no", 0);
            doc.Element("bankdata").Element("bank").SetAttributeValue("comment", "305405B000");
            doc.Element("bankdata").Element("bank").SetAttributeValue("wirename", "ASSSH2SH050B");
            doc.Element("bankdata").Element("bank").SetAttributeValue("wirelength", "500");
            doc.Element("bankdata").Element("bank").SetAttributeValue("strip1", "40.0");
            doc.Element("bankdata").Element("bank").SetAttributeValue("strip2", "40.0");
        }

    }
}
