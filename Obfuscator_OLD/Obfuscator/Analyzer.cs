using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Obfuscator.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Obfuscator.Common.FileReader;


namespace Obfuscator
{
    public abstract class AnalyzedData
    {
        public UsingDirectiveSyntax[]? usingDirectives { get; private protected set; }
        public NamespaceDeclarationSyntax[]? namespaces { get; private protected set; }
        public AttributeSyntax[]? attributes { get; private protected set; }
        public DelegateDeclarationSyntax[]? delegates { get; private protected set; }
        public EventDeclarationSyntax[]? events { get; private protected set; }
        public InterfaceDeclarationSyntax[]? interfaces { get; private protected set; }
        public EnumDeclarationSyntax[]? enums { get; private protected set; }
        public ClassDeclarationSyntax[]? classes { get; private protected set; }
        public StructDeclarationSyntax[]? structs { get; private protected set; }
        public RecordDeclarationSyntax[]? records { get; private protected set; }
        public ConstructorDeclarationSyntax[]? constructors { get; private protected set; }
        public DestructorDeclarationSyntax[]? destructors { get; private protected set; }
        public MethodDeclarationSyntax[]? methods { get; private protected set; }
        public VariableDeclarationSyntax[]? variables { get; private protected set; }
    }

    public sealed class Analyzer : AnalyzedData
    {
        private FileReader? _fileReader;
        public Analyzer(FileReader fileReader)
        {
            if (fileReader == null || fileReader?.InputFiles?.Count < 1) 
            {
               // var frame = GetStackTrace();
                //throw new NullReferenceException($"FileReader is null: {frame}");
            }

            this._fileReader = fileReader;
        }

        ~Analyzer()
        {
            this._fileReader = null;
        }


        [Flags]
        public enum AnalyzationTypes
        {
            UsingDirective,
            Namespace,
            Attribute,
            Delegate,
            Event,
            Interface,
            Enum,
            Class,
            Struct,
            Record,
            Method,
            Variable,
            ALL = UsingDirective | Namespace | Attribute | Delegate | Event | Interface | Enum | Class | Struct | Record | Method | Variable,
        }



        public void Analyze(string filePath, AnalyzationTypes types = AnalyzationTypes.ALL)
        {
            /*
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            if(FlagManager<AnalyzationTypes>.Has(types, AnalyzationTypes.Method)) { methods = GetMethods(filePath)?.ToArray(); }
            */
        }


        public IEnumerable<MethodDeclarationSyntax>? GetMethods(string filePath) => GetSyntaxNode<MethodDeclarationSyntax>(filePath);
        private IEnumerable<NamespaceDeclarationSyntax>? GetNamespaces(string filePath) => GetSyntaxNode<NamespaceDeclarationSyntax>(filePath);
        private IEnumerable<UsingDirectiveSyntax>? GetUsingDirectives(string filePath) => GetSyntaxNode<UsingDirectiveSyntax>(filePath);
        private IEnumerable<VariableDeclarationSyntax>? GetVariables(string filePath) => GetSyntaxNode<VariableDeclarationSyntax>(filePath);
        private IEnumerable<ClassDeclarationSyntax>? GetClasses(string filePath) => GetSyntaxNode<ClassDeclarationSyntax>(filePath);
        private IEnumerable<ConstructorDeclarationSyntax>? GetConstructors(string filePath) => GetSyntaxNode<ConstructorDeclarationSyntax>(filePath);
        private IEnumerable<DelegateDeclarationSyntax>? GetDelegates(string filePath) => GetSyntaxNode<DelegateDeclarationSyntax>(filePath);
        private IEnumerable<DestructorDeclarationSyntax>? GetDestructors(string filePath) => GetSyntaxNode<DestructorDeclarationSyntax>(filePath);
        private IEnumerable<EnumDeclarationSyntax>? GetEnums(string filePath) => GetSyntaxNode<EnumDeclarationSyntax>(filePath);
        private IEnumerable<StructDeclarationSyntax>? GetStructs(string filePath) => GetSyntaxNode<StructDeclarationSyntax>(filePath);
        private IEnumerable<RecordDeclarationSyntax>? GetRecords(string filePath) => GetSyntaxNode<RecordDeclarationSyntax>(filePath);
        private IEnumerable<EventDeclarationSyntax>? GetEvents(string filePath) => GetSyntaxNode<EventDeclarationSyntax>(filePath);
        private IEnumerable<InterfaceDeclarationSyntax>? GetInterfaces(string filePath) => GetSyntaxNode<InterfaceDeclarationSyntax>(filePath);
        private IEnumerable<AttributeSyntax>? GetAttributes(string filePath) => GetSyntaxNode<AttributeSyntax>(filePath);

        private IEnumerable<TSyntax>? GetSyntaxNode<TSyntax>(string filePath) where TSyntax : CSharpSyntaxNode
        {
            string? code = this._fileReader?.GetContent(filePath);
            if (String.IsNullOrEmpty(code)) { return null; }

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            if(syntaxTree?.GetDiagnostics()?.Count() > 0) { throw new FormatException($"Diagnostics detected incorrect syntax!\n{filePath}"); }

            return syntaxTree?.GetCompilationUnitRoot()?.DescendantNodes()?.OfType<TSyntax>();
        }
    }
}