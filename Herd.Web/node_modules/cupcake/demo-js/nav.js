  $(document).ready(function() {

    // sidebar nav elements
    var $sidebarNavContainer = $(".c-sidebar-nav")
    var $sidebarNavBody = $(".c-sidebar-nav-body");
    var $sidebarNavListToggle = $(".c-sidebar-nav-list-toggle");
    var $sidebarNavLinkName =$(".c-sidebar-nav-link");
    var $sidebarNavChildList = ".c-sidebar-nav-list";
    var $sidebarNavToggle = $(".c-sidebar-nav-toggle");

    // sidebar nav sub menu toggle
    $sidebarNavListToggle.on("click", function() {
      $(this).find(".fa").toggleClass("fa-caret-right fa-caret-down");
      $(this).siblings($sidebarNavChildList).slideToggle(200);
      $(this).parent().toggleClass("open");
    })


    // sidebar navigation open/close toggle
    $sidebarNavToggle.on("click", function() {
      $(this).find(".fa").toggleClass("fa-angle-double-left fa-angle-double-right");
      $sidebarNavContainer.toggleClass("c-sidebar-nav-closed");
    })

  })
