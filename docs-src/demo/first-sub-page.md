<!--
title: "First Sub Page"
template: "_sub-page"
next: "Second Sub Page"
source: "https://github.com/e-wipond/Gen/blob/master/docs-src/demo/first-sub-page.md"
-->

Source files can have frontmatter that specifies additional information about how to render them in the generated site. The frontmatter is formatted as YAML in an HTML comment at the top of the file.

This page and its siblings use a custom template called `_sub-page` that provides the navigation links, title, and source file link. The template is specified in the frontmatter like `template: _sub-page`. See the source of the template [here](https://github.com/e-wipond/Gen/blob/master/docs-src/demo/_sub-page.md).

The title heading is rendered via variable replacement. Define a variable in the frontmatter like `title: First Sub Page` and use it in a template like by wrapping it in double curly braces (`{{``title``}}`).

Breadcrumb navigation can be inserted with `{{``breadcrumbs``}}` and creates links back to the site root.

