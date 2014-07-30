using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alchemist
{
    public class SystemXMLAccessor : XMLAccessor
    {
        private const string schema = @"
<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
	<!-- borate_string型 -->
	<xsd:simpleType name='borate_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='4800' />
			<xsd:enumeration value='9600' />
			<xsd:enumeration value='14400' />
			<xsd:enumeration value='19200' />
			<xsd:enumeration value='38400' />
			<xsd:enumeration value='57600' />
			<xsd:enumeration value='115200' />
		</xsd:restriction>
	</xsd:simpleType>
	
	<!-- databits_type -->
	<xsd:simpleType name='databit_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='7' />
			<xsd:enumeration value='8' />
		</xsd:restriction>
	</xsd:simpleType>	

	<!-- stopbit_string -->
	<xsd:simpleType name='stopbit_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='1' />
			<xsd:enumeration value='2' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- parity_string -->
	<xsd:simpleType name='parity_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='none' />
			<xsd:enumeration value='even' />
			<xsd:enumeration value='odd' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- flow_string -->
	<xsd:simpleType name='flow_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='none' />
			<xsd:enumeration value='hard' />
			<xsd:enumeration value='xonxoff' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- culture_string -->
	<xsd:simpleType name='culture_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='ja-JP' />
			<xsd:enumeration value='en-US' />
			<xsd:enumeration value='zh-CN' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- machineoperation_string -->
	<xsd:simpleType name='machineoperation_string'>
		<xsd:restriction base='xsd:string'>
			<xsd:enumeration value='machine' />
			<xsd:enumeration value='both' />
		</xsd:restriction>
	</xsd:simpleType>

	<!-- system要素定義 -->
	<xsd:element name='system' type='system_type' />
	<xsd:complexType name='system_type'>
        <xsd:sequence>
		    <xsd:element name='communication' type='communication_type' />
		    <xsd:element name='locale' type='locale_type' />
			<xsd:element name='password' type='password_type' />
			<xsd:element name='machineoperation' type='machineoperation_type' />
            <xsd:element name='machineid' type='machieid_type' />
            <xsd:element name='tachpanel' type='tachpanel_type' />
            <xsd:element name='sqlserver' type='sqlserver_type' />
        </xsd:sequence>
	</xsd:complexType>


	<!-- communication要素定義 -->
	<xsd:complexType name='communication_type'>
		 <!-- 空要素 -->
        <xsd:sequence />

		<!-- COM属性定義 -->
		<xsd:attribute name='comport' use='required' type='xsd:string' />
		
		<!-- borate属性定義 -->
		<xsd:attribute name='borate' use='required' type='borate_string' />
		
		<!-- databit属性定義 -->
		<xsd:attribute name='databit' use='required' type='databit_string' />

		<!-- databit属性定義 -->
		<xsd:attribute name='stopbit' use='required' type='stopbit_string' />

		<!-- parity属性定義 -->
		<xsd:attribute name='parity' use='required' type='parity_string' />

		<!-- flow属性定義 -->
		<xsd:attribute name='flow' use='required' type='flow_string' />
	</xsd:complexType>

	<!-- locale要素定義 -->
	<xsd:complexType name='locale_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- culture属性 -->
		<xsd:attribute name='culture' use='required' type='culture_string' />
	</xsd:complexType>

	<!-- machineoperation要素定義 -->
	<xsd:complexType name='machineoperation_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- type属性 -->
		<xsd:attribute name='type' use='required' type='machineoperation_string' />
	</xsd:complexType>

	<!-- password要素定義 -->
	<xsd:complexType name='password_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- value属性 -->
		<xsd:attribute name='value' use='required' type='xsd:string' />
	</xsd:complexType>

	<!-- machineid要素定義 -->
	<xsd:complexType name='machieid_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- value属性 -->
		<xsd:attribute name='value' use='required' type='xsd:string' />
    </xsd:complexType>

	<!-- tachpanel要素定義 -->
	<xsd:complexType name='tachpanel_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- value属性 -->
		<xsd:attribute name='value' use='required' type='xsd:string' />
	</xsd:complexType>

	<!-- sqlserver要素定義 -->
	<xsd:complexType name='sqlserver_type'>
        <!-- 空要素 -->
        <xsd:sequence />

		<!-- machinename属性 -->
		<xsd:attribute name='machinename' use='required' type='xsd:string' />

		<!-- databasename属性 -->
        <xsd:attribute name='databasename' use='required' type='xsd:string' />

		<!-- userid属性 -->
        <xsd:attribute name='userid' use='required' type='xsd:string' />

		<!-- password属性 -->
        <xsd:attribute name='password' use='required' type='xsd:string' />
	</xsd:complexType>
</xsd:schema>
";
        protected override string GetSchema()
        {
            return schema;
        }
    }
}
