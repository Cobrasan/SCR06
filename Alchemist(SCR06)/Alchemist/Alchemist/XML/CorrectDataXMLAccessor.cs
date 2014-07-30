using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Alchemist
{
    public class CorrectDataXMLAccessor : XMLAccessor
    {
		private const string schema = @"
<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
	<!-- workid_type -->
	<xsd:simpleType name='type_number'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='2' />
			<xsd:enumeration value='3' />
		</xsd:restriction>
	</xsd:simpleType>	

	<!-- address_string -->
	<xsd:simpleType name='address_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:pattern value='^0x[0-9A-F]{4}'/>
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

	<!-- correctdata要素定義 -->
	<xsd:element name='correctdata' type='correctdata_type'>
		<!-- 主キー定義 -->
		<xsd:unique name='pkEntry'>
			<xsd:selector xpath='entry' />
			<xsd:field xpath='@type' />
			<xsd:field xpath='@workid' />
		</xsd:unique>
	</xsd:element>

	<xsd:complexType name='correctdata_type'>
        <xsd:sequence>
		    <xsd:element name='entry' minOccurs='0' maxOccurs='unbounded' type='entry_type' />
        </xsd:sequence>
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
                new XElement("correctdata")
            );
        }
    }
}
