using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemist
{
    public class MemallocXmlAccessor : XMLAccessor
    {
        private const string schema = @"
<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
	<!-- workid_string型 -->
	<xsd:simpleType name='workid_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='1' />
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

	<!-- double_word_string -->
	<xsd:simpleType name='double_word_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='0' />
			<xsd:enumeration value='1' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- value_factor_string -->
	<xsd:simpleType name='value_factor_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='1' />
			<xsd:enumeration value='10' />
			<xsd:enumeration value='100' />
			<xsd:enumeration value='1000' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- bank_store_string -->
	<xsd:simpleType name='bank_store_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='0' />
			<xsd:enumeration value='1' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- address_special_string -->
	<xsd:simpleType name='address_special_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='0' />
			<xsd:enumeration value='1' />
            <xsd:enumeration value='2' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- memalloc要素定義 -->
	<xsd:element name='memalloc' type='memalloc_type' />
	<xsd:complexType name='memalloc_type'>
        <xsd:sequence>
		    <xsd:element name='machine' type='machine_type' />
		    <xsd:element minOccurs='1' maxOccurs='unbounded' name='workid_type' type='workid_type_type'>
				<!-- 主キー定義 -->
				<xsd:unique name='pkGroup'>
					<xsd:selector xpath='group' />
					<xsd:field xpath='@code' />
				</xsd:unique>
			</xsd:element>
        </xsd:sequence>
	</xsd:complexType>


	<!-- machine要素定義 -->
	<xsd:complexType name='machine_type'>
		 <!-- 空要素 -->
        <xsd:sequence />
		
		<!-- id属性定義 -->
		<xsd:attribute name='id' use='required' type='xsd:string' />
	</xsd:complexType>

	<!-- workid_type要素定義 -->
	<xsd:complexType  name='workid_type_type'>
        <xsd:sequence>
			<xsd:element minOccurs='1' maxOccurs='unbounded' name='group' type='group_type'>
				<!-- 主キー定義 -->
				<xsd:unique name='pkWorkID'>
					<xsd:selector xpath='workid' />
					<xsd:field xpath='@id' />
				</xsd:unique>
			</xsd:element>
		</xsd:sequence>

		<!-- type属性 -->
		<xsd:attribute name='type' use='required' type='workid_string' />
	</xsd:complexType>

	<!-- group要素定義 -->
	<xsd:complexType name='group_type'>
        <xsd:sequence>
			<xsd:element minOccurs='1' maxOccurs='unbounded' name='workid' type='workid_type' />
		</xsd:sequence>

		<!-- code属性 -->
		<xsd:attribute name='code' use='required' type='xsd:integer' />
	</xsd:complexType>

	<!-- workid要素定義 -->
	<xsd:complexType name='workid_type'>
        <!-- 空要素 -->
        <xsd:sequence />
			
		<xsd:attribute name='id' use='required' type='address_string' />
		<xsd:attribute name='sortno' use='required' type='xsd:integer' />
		<xsd:attribute name='address' use='required' type='address_string' />
		<xsd:attribute name='double_word' use='required' type='double_word_string' />
		<xsd:attribute name='value_factor' use='required' type='value_factor_string' />
		<xsd:attribute name='bank_store' use='required' type='bank_store_string' />
		<xsd:attribute name='min_limit' use='required' type='xsd:double' />
		<xsd:attribute name='max_limit' use='required' type='xsd:double' />
		<xsd:attribute name='default_value' use='required' type='xsd:double' />
		<xsd:attribute name='unit' use='required' type='xsd:string' />
        <xsd:attribute name='address_special' use='required' type='address_special_string' />
		<xsd:attribute name='comment' use='required' type='xsd:string' />
	</xsd:complexType>
</xsd:schema>
";
        protected override string GetSchema()
        {
            return schema;
        }
    }
}
