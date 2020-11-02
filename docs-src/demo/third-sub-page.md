<!--
title: "Third Sub Page"
template: "_sub-page"
prev: "Second Sub Page"
source: "https://github.com/e-wipond/Gen/blob/master/docs-src/demo/third-sub-page.md"
-->

Syntax highlighting is implemented for Scheme snippets.

```scheme
(define (sqrt x)
  (define (good-enough? guess)
    (< (abs (- (square guess) x)) 0.001))
  (define (improve guess)
    (average guess (/ x guess)))
  (define (sqrt-iter guess)
    (if (good-enough? guess)
        guess
        (sqrt-iter (improve guess))))
  (sqrt-iter 1.0))
```


