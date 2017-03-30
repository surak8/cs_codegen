using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NSCs_codegen {
    public partial class cs_codegenDriver {

        #region fields
        static readonly CodeStatement csBlank = new CodeSnippetStatement();
        static readonly CodeStatement csBreak = new CodeSnippetStatement("\t\t\tbreak;");

        static readonly CodeExpression ceThis = new CodeThisReferenceExpression();
        static readonly CodeExpression ceValue = new CodePropertySetValueReferenceExpression();
        static readonly CodeExpression ceTrue = new CodePrimitiveExpression(true);
        static readonly CodeExpression ceZero = new CodePrimitiveExpression(0);
        static readonly CodeExpression ceOne = new CodePrimitiveExpression(1);

        public static bool showCode = false;
        static IDictionary<string, string> nameMap = new Dictionary<string, string>();
        #endregion

        static void generateCodeFromProcedure(IDbConnection conn, string procName, string outputPath, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts) {
        SqlDataReader reader;

            try {
                using (SqlCommand cmd = new SqlCommand(procName, (SqlConnection)conn)) {
                    reader = cmd.ExecuteReader();
                    generateStuff(makeClassName(procName), nameSpace, outputPath, cdp, opts, reader,procName);
                    reader.Close();
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }
        }

        static void generateStuff(string className, string outDir, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts, SqlDataReader reader,string tableName) {
            CodeCompileUnit ccu;
            CodeNamespace ns, ns0;
            CodeTypeDeclaration ctd;
            string tmp, fldName, propName;
            Type colType;


            CodeMemberProperty p;
            CodeStatementCollection csc, csc2;

            CodeVariableReferenceExpression vr, vrI;
            CodeArgumentReferenceExpression ar;
            CodeConstructor cc;
            CodeStatement csCN;
            CodeStatement csswitch;
            CodeIterationStatement cis;
#if FIELD_AND_PROPERTY
            CodeMemberField f;
           CodeMemberProperty firstProp = null;
            CodeMemberField firstField = null;

            p = null;
            f = null;
#endif
              if (string.IsNullOrEmpty(outDir))
                    outDir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

     
          
            //    foreach()
            ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(ns = ns0 = new CodeNamespace());
            ns0.Imports.Add(new CodeNamespaceImport("System"));
            ns0.Imports.Add(new CodeNamespaceImport("System.Data"));
            if (!string.IsNullOrEmpty(nameSpace))
                ccu.Namespaces.Add(ns = new CodeNamespace(nameSpace));
            ns.Types.Add(ctd = new CodeTypeDeclaration(className));
            ctd.IsPartial = true;

            generateCSVFrom(reader, tableName, outDir,ctd);

            ar = new CodeArgumentReferenceExpression("reader");
            cc = new CodeConstructor();
            cc.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "ctor"));
            cc.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "ctor"));
            vr = new CodeVariableReferenceExpression("colName");
            vrI = new CodeVariableReferenceExpression("i");
            csc = new CodeStatementCollection();
#if true
            csCN = new CodeAssignStatement(vr,
                new CodeMethodInvokeExpression(ar, "GetName", vrI));
#else
            csCN = new CodeExpressionStatement(
                new CodeBinaryOperatorExpression(vr, CodeBinaryOperatorType.Assign,
                new CodeMethodInvokeExpression(ar, "GetName", vrI)));
//                ))
#endif

            csc2 = new CodeStatementCollection();
            csswitch = new CodeSnippetStatement("\t\t\tswitch(" + removeTrailingChar(generate(csCN, cdp, opts), ';') + ") {");
            csc2.Add(csswitch);

            //         CodeExpression ceInc;
            //        string tmp;

            tmp = removeParensFrom(generate(new CodeExpressionStatement(
                                new CodeBinaryOperatorExpression(vrI,
                                    CodeBinaryOperatorType.Assign, new CodeBinaryOperatorExpression(vrI, CodeBinaryOperatorType.Add, ceOne))), cdp, opts));
            csc.AddRange(new CodeStatement[] {
                        new CodeVariableDeclarationStatement(typeof(string),vr.VariableName),
                        csBlank,
                        cis=new CodeIterationStatement(
                            new CodeVariableDeclarationStatement(typeof(int),vrI.VariableName,ceZero),
                            new CodeBinaryOperatorExpression (vrI,  CodeBinaryOperatorType.LessThan ,
                            new CodePropertyReferenceExpression (ar,"FieldCount")),
                            new CodeSnippetStatement (tmp))

                    });
            for (int i = 0; i < reader.FieldCount; i++) {
                tmp = reader.GetName(i);
                fldName = makeFieldName(tmp);
                propName = makePropName(tmp);
                colType = reader.GetFieldType(i);
#if FIELD_AND_PROPERTY
                ctd.Members.AddRange(
                    new CodeTypeMember[] {
                        f=makeField(fldName ,colType),
                        p=makeProperty(propName,f)
                    });
                if (firstProp == null)
                    firstProp = p;
                if (firstField == null)
                    firstField = f;
#else
#if true
                ctd.Members.Add(new CodeSnippetTypeMember("\t\tpublic " + cdp.GetTypeOutput(new CodeTypeReference(colType)) + " " + propName + " { get; set; }\r\n"));
#else
                ctd.Members.Add(p = new CodeMemberProperty());
                p.Name = propName;
                p.Type = new CodeTypeReference(colType);
                p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                p.HasGet = true;
                p.HasSet = true;
#endif
#endif
                addCaseStatementsTo(csc2, tmp,
                    new CodePropertyReferenceExpression(ceThis, propName),
                    colType, ar, vrI, cdp, opts);
            }

#if FIELD_AND_PROPERTY
            firstProp.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "properties"));
            p.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "properties"));

            firstField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "fields"));
            f.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, "fields"));
#endif

            csc2.Add(new CodeSnippetStatement("\t\t}"));
            cis.Statements.AddRange(csc2);
            ctd.Members.Add(cc);
            cc.Statements.AddRange(csc);
            cc.Parameters.Add(new CodeParameterDeclarationExpression("IDataReader", ar.ParameterName));
            if (ccu != null) {

             StringBuilder sb;
                string fname = Path.Combine(outDir, className + "." + cdp.FileExtension);
                using (StringWriter sw = new StringWriter(sb = new StringBuilder())) {
                    //                    Trace.WriteLine("here");
                    cdp.GenerateCodeFromCompileUnit(ccu, sw, opts);
                }
                if (showCode)
                    Trace.WriteLine("Code is:" + Environment.NewLine + sb.ToString());
                File.WriteAllText(fname, sb.ToString());
            }
        }

        static void generateCSVFrom(SqlDataReader reader, string tableName, string outDir, CodeTypeDeclaration ctd) {
            DataTable dt = reader.GetSchemaTable();
            int i0 = 0;
            string filename;

            using (TextWriter tw = new StreamWriter(filename = Path.Combine(outDir, tableName + ".csv"))) {
                foreach (DataColumn dc in dt.Columns) {
                    if (i0 > 0)
                        tw.Write(",");
                    tw.Write(dc.ColumnName);
                    i0++;
                }
                tw.WriteLine();

                foreach (DataRow dr in dt.Rows) {
                    i0 = 0;
                    foreach (DataColumn dc in dt.Columns) {
                        if (i0 > 0)
                            tw.Write(",");
                        tw.Write(dr[dc]);
                        i0++;
                    }
                    tw.WriteLine();
                }
                tw.WriteLine();
            }
            Debug.Print("generated: " + filename);
            
            CodeTypeReference ctrColDef = new CodeTypeReference("ColumnDef");
            CodeExpression[] args = makeClassFieldCollection(dt, ctrColDef);
            CodeMemberField f;
            ctd.Members.Add(f = new CodeMemberField());
            f.Attributes = MemberAttributes.Static;
            f.Name = "_fields";
            var avar = new CodeTypeParameter("string");
            f.Type = new CodeTypeReference("List", ctrColDef);
            f.InitExpression = new CodeObjectCreateExpression(f.Type, new CodeArrayCreateExpression(ctrColDef, args));
        }

        static CodeExpression[] makeClassFieldCollection(DataTable dt, CodeTypeReference ctrColDef) {
            List<CodeExpression> exprs = new List<CodeExpression>();
            /*
             * ColumnName,
             * ColumnOrdinal,
             * ColumnSize,
             * IsUnique? (not working)
             * DataType,
             * AllowDBNull,
             * DataTypeName
             * */
            CodeObjectCreateExpression ret = new CodeObjectCreateExpression(ctrColDef);
            foreach (DataRow dr in dt.Rows) {
                exprs.Add(ret = new CodeObjectCreateExpression(ctrColDef));
                ret.Parameters.Add(new CodePrimitiveExpression(dr["ColumnName"].ToString()));
                //exprs.Add(createColDef(ctrColDef, "columnName", dr["ColumnName"].ToString()));
                //exprs.Add(createColDef(ctrColDef, "columnIndex", Convert.ToInt32 (dr["ColumnOrdinal"].ToString())));
                //exprs.Add(createColDef(ctrColDef, "columnSize", Convert.ToInt32(dr["ColumnSize"])));
                //exprs.Add(createColDef(ctrColDef, "DataType", dr["DataType"].ToString()));
                //exprs.Add(createColDef(ctrColDef, "AllowDBNull", Convert.ToBoolean(dr["AllowDBNull"])));
                //exprs.Add(createColDef(ctrColDef, "DataTypeName", dr["DataTypeName"].ToString()));
            }
        //    return new CodeExpression[] { ret };
            //exprs.Add(createColDef(ctrColDef, "columnName", "col1"));
            //exprs.Add(
            //new CodeObjectCreateExpression(ctrColDef,
            //            new CodeBinaryOperatorExpression(
            //                new CodeSnippetExpression("columnName"),
            //                CodeBinaryOperatorType.Assign,
            //                new CodeSnippetExpression("\"dummy\""))));
            //),


            //        new CodeObjectCreateExpression(ctrColDef),
            //        new CodeObjectCreateExpression(ctrColDef),
            //        new CodeObjectCreateExpression(ctrColDef),
            //        new CodeObjectCreateExpression(ctrColDef)));


            return exprs.ToArray();
        }

        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, string fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodeSnippetExpression("\"" + fldValue + "\"")));
        }
        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, int fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodePrimitiveExpression(fldValue)));
        }
        static CodeExpression createColDef(CodeTypeReference ctr, string fldName, bool fldValue) {
            return new CodeObjectCreateExpression(
                ctr,
                new CodeBinaryOperatorExpression(
                    new CodeSnippetExpression(fldName),
                    CodeBinaryOperatorType.Assign,
                    new CodePrimitiveExpression(fldValue)));
        }

        static CodeTypeReference makeTypeReference(Type t, CodeDomProvider cdp) {
            string tmp = cdp.GetTypeOutput(new CodeTypeReference(t));

            return new CodeTypeReference("dummy");
        }

        static string removeParensFrom(string v) {
            string tmp2;
            StringBuilder sb = new StringBuilder();
            string[] blah = v.Split('\r', '\n', '(', ')', ';');

            foreach (string str in blah)
                if (!string.IsNullOrEmpty(tmp2 = str.Trim()))
                    sb.Append(tmp2);
            return sb.ToString();
        }

        static string removeTrailingChar(string v1, char v2) {
            if (!string.IsNullOrEmpty(v1) && v2 != char.MinValue) {
                //               Logger.logMethod(MethodBase.GetCurrentMethod());
                StringBuilder sb = new StringBuilder();
                String tmp2;
                string[] blah = v1.Split('\r', '\n', v2);

                foreach (string str in blah)
                    if (!string.IsNullOrEmpty(tmp2 = str.Trim()))
                        sb.Append(tmp2);
                return sb.ToString();

                //     return v1.Replace(v2, char.MinValue);
            }
            return v1;
        }

        static void addCaseStatementsTo(CodeStatementCollection csc, string fieldName, CodeExpression propRef, Type colType,
            CodeArgumentReferenceExpression ar, CodeVariableReferenceExpression vrI, CodeDomProvider cdp, CodeGeneratorOptions opts) {

            csc.AddRange(new CodeStatement[] {
                new CodeSnippetStatement("\t\tcase \"" + fieldName + "\":"),
                new CodeAssignStatement(propRef, makeNullAssignment(colType, cdp)),
                new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeMethodInvokeExpression(ar, "IsDBNull", vrI), CodeBinaryOperatorType.IdentityInequality, ceTrue),
                        new CodeCommentStatement("do real assign here"),
                        new CodeAssignStatement(propRef, new CodeMethodInvokeExpression(ar, findTypeMethod(colType, cdp), vrI))),
                csBreak
            });
        }

        static CodeExpression makeNullAssignment(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return new CodeSnippetExpression(outType + ".Empty");
            else if (string.Compare(outType, "int", true) == 0) return new CodeSnippetExpression("Int32.MinValue");
            else if (string.Compare(outType, "long", true) == 0) return new CodeSnippetExpression("Int64.MinValue");
            else if (string.Compare(outType, "bool", true) == 0) return new CodeSnippetExpression("false");
            else if (string.Compare(outType, "decimal", true) == 0) return new CodeSnippetExpression("Decimal.MinValue");
            else if (string.Compare(outType, "System.Guid", true) == 0) return new CodeSnippetExpression("Guid.Empty");
            else if (string.Compare(outType, "System.DateTime", true) == 0) return new CodeSnippetExpression("DateTime.MinValue");
            throw new InvalidOperationException("Unhandled type: " + colType.FullName);
            //            return new CodeSnippetExpression(outType + ".MinValue");
        }

        static string findTypeMethod(Type colType, CodeDomProvider cdp) {
            string outType = cdp.GetTypeOutput(new CodeTypeReference(colType));

            if (string.Compare(outType, "string", true) == 0) return "GetString";
            else if (string.Compare(outType, "int", true) == 0) return "GetInt32";
            else if (string.Compare(outType, "long", true) == 0) return "GetInt64";
            else if (string.Compare(outType, "bool", true) == 0) return "GetBoolean";
            else if (string.Compare(outType, "decimal", true) == 0) return "GetDecimal";
            else if (string.Compare(outType, "System.Guid", true) == 0) return "GetGuid";
            else if (string.Compare(outType, "System.DateTime", true) == 0) return "GetDateTime";
            throw new InvalidOperationException("Unhandled type: " + colType.FullName);
            //          return "UnhandledType_" + outType;

        }

        static string generate(CodeStatement cs, CodeDomProvider cdp, CodeGeneratorOptions opts) {
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb)) {
                cdp.GenerateCodeFromStatement(cs, sw, opts);
            }
            return sb.ToString();
        }

        static CodeMemberProperty makeProperty(string propName, CodeMemberField f) {
            CodeMemberProperty p = new CodeMemberProperty();
            CodeFieldReferenceExpression fr = new CodeFieldReferenceExpression(ceThis, f.Name);

            p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            p.Name = propName;
            p.Type = f.Type;

            p.SetStatements.Add(new CodeAssignStatement(fr, ceValue));
            p.GetStatements.Add(new CodeMethodReturnStatement(fr));
            return p;
        }

        static CodeMemberField makeField(string fldName, Type colType) {
            CodeMemberField f = new CodeMemberField(makeTypeRef(colType), fldName);

            f.Attributes = 0;
            return f;
        }

        static CodeTypeReference makeTypeRef(Type colType) {
            return new CodeTypeReference(colType.FullName);
        }

        static string makePropName(string tmp) {
            return fixup(tmp.Replace(' ', '_'));
        }

        static string makeFieldName(string tmp) {
            return "_" + fixup(tmp.Replace(' ', '_'));
        }


        static string fixup(string procName) {
            return fixup(procName, false);
        }
        static string fixup(string procName, bool isClass) {
            StringBuilder sb;
            int len;
            bool bupper;
            char c;

            if (string.IsNullOrEmpty(procName))
                throw new ArgumentNullException("procName", "string is null!");
            if (!nameMap.ContainsKey(procName)) {
                sb = new StringBuilder();
                len = procName.Length;
                if (isClass)
                    sb.Append(char.ToUpper(procName[0]), 1);
                else
                    sb.Append(char.ToLower(procName[0]), 1);
                bupper = false;
                for (int i = 1; i < len; i++)
                    if ((c = procName[i]) == '_')
                        bupper = true;
                    else {
                        if (char.IsWhiteSpace(c))
                            sb.Append('_', 1);
                        else {
                            if (bupper) {
                                bupper = false;
                                sb.Append(char.ToUpper(c), 1);
                            } else
                                sb.Append(char.ToLower(c), 1);
                        }
                    }
                nameMap.Add(procName, sb.ToString());
            }
            return nameMap[procName];
        }

        static string makeClassName(string procName) {
#if true
            return fixup(procName, true);
#else
            return procName.Substring(0, 1).ToUpper() +
                procName.Substring(1).ToLower();
#endif
        }
    }
}