namespace LinqToService.ExpressionTrees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.UI;

    internal class TreeVisualizer : ExpressionVisitor
    {
        private StringBuilder builder;
        private HtmlTextWriter writer;
        private StringBuilder trace;

        private TreeVisualizer()
        {
            this.builder = new StringBuilder();
            this.writer = new HtmlTextWriter(
                new StringWriter(
                    this.builder,
                    CultureInfo.InvariantCulture),
                "    ");
            this.trace = new StringBuilder();
        }

        public string Visualization
        {
            get
            {
                return this.builder.ToString();
            }
        }

        public string Trace
        {
            get
            {
                return this.trace.ToString();
            }
        }

        internal static string BuildVisualization(Expression expression)
        {
            TreeVisualizer visualizer = new TreeVisualizer();
            visualizer.Visit(expression);
            return visualizer.Visualization;
        }

        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                this.trace.AppendLine(string.Format("Visit : {0}", node.NodeType));
            }
            else
            {
                this.trace.AppendLine("Visit : null");
            }
            return base.Visit(node);
        }

        #region VisitSomeTypeOfNode

        protected override Expression VisitBinary(
            BinaryExpression node)
        {
            return VisitAndBuildTree(
                "Binary",
                string.Empty,
                node.NodeType.ToString(),
                () => base.VisitBinary(node));
        }

        protected override Expression VisitConstant(
            ConstantExpression node)
        {
            string type = GetSimplifiedType(node.Type);

            string value;
            if (node.Type.IsGenericType
                && node.Type.FindInterfaces(
                    (t, o) => t.Name.StartsWith("IEnumerable"),
                    true).Any())
            {
                value = type;
            }
            else if (type == "String")
            {
                value = string.Concat(
                    "\"",
                    ((string)node.Value).Replace("\"", "\\\""),
                    "\"");
            }
            else
            {
                value = node.Value.ToString();
            }

            VisitAndBuildTree(
                "Constant",
                type,
                value);
            return base.VisitConstant(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return VisitAndBuildTree(
                "Lambda",
                GetSimplifiedType(node.Type),
                node.ToString(),
                () => base.VisitLambda<T>(node));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return VisitAndBuildTree(
                "Member",
                GetSimplifiedType(node.Type),
                node.Member.Name,
                () => base.VisitMember(node));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return VisitAndBuildTree(
                "Call",
                GetSimplifiedType(node.Type),
                node.Method.Name,
                () => base.VisitMethodCall(node));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            VisitAndBuildTree(
                "Parameter",
                GetSimplifiedType(node.Type),
                node.Name);
            return base.VisitParameter(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        #endregion

        private Expression VisitAndBuildTree(
            string nodeName,
            string nodeType,
            string nodeDescription,
            Func<Expression> childrenVisitorFunction = null)
        {
            this.writer.RenderBeginTag("li");
            this.writer.WriteLine();
            this.writer.Indent++;

            this.writer.AddAttribute("href", "#");
            this.writer.RenderBeginTag("a");

            this.writer.AddAttribute("class", "node-name");
            this.writer.RenderBeginTag("span");
            this.writer.WriteEncodedText(nodeName);
            this.writer.RenderEndTag();
            this.writer.WriteBreak();

            if (!string.IsNullOrEmpty(nodeType))
            {
                this.writer.AddAttribute("class", "node-type");
                this.writer.RenderBeginTag("span");
                this.writer.WriteEncodedText(nodeType);
                this.writer.RenderEndTag();
                this.writer.WriteBreak();
            }

            this.writer.WriteEncodedText(nodeDescription);
            this.writer.RenderEndTag();
            this.writer.WriteLine();

            Expression baseReturn = null;
            if (childrenVisitorFunction != null)
            {
                this.writer.RenderBeginTag("ul");
                this.writer.WriteLine();
                this.writer.Indent++;

                baseReturn = childrenVisitorFunction();

                this.writer.Indent--;
                this.writer.RenderEndTag();
                this.writer.WriteLine();

            }

            this.writer.Indent--;
            this.writer.RenderEndTag();
            this.writer.WriteLine();

            return baseReturn;
        }

        private string GetSimplifiedType(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            string genericName = type.Name.Split('`').First();

            string genericArguments = string.Join(
                ", ",
                type.GenericTypeArguments.Select(
                    t => GetSimplifiedType(t)));

            return string.Format(
                "{0}<{1}>",
                genericName,
                genericArguments);
        }
    }
}
