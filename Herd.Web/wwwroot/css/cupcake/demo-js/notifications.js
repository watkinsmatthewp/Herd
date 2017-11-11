  $(document).ready(function() {
    // Show Notifications panel
    $("#show-notifications").on("click", function() {

      $(".chat-pane").removeClass("open");
      $("#show-chats").parent().removeClass("open");

      $(this).parent().toggleClass("c-notification-open");
      // $(".notification-pane").toggle();
      $(".c-notification").toggleClass("c-notification-open");
    })
    // Notifications scroll - prevent body from scrolling
    $('.c-notification-body').on('mousewheel DOMMouseScroll', function(e) {
      var scrollTo = null;
      if (e.type == 'mousewheel') {
        scrollTo = (e.originalEvent.wheelDelta * -1);
      }
      else if (e.type == 'DOMMouseScroll') {
        scrollTo = 40 * e.originalEvent.detail;
      }
      if (scrollTo) {
        e.preventDefault();
        $(this).scrollTop(scrollTo + $(this).scrollTop());
      }
    });
  });
